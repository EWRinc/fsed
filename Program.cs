using System;

namespace fsed
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("fsed [-i] [-o] -l <linenumber> -c <Character offset> <text>");
                Console.WriteLine(" -i : Insert mode");
                Console.WriteLine(" -o : Overwrite mode (default)");
                return;
            }
            
            //Argument parsing
            int current_linenumber = 1;
            int current_offset = 1;
            intrange dest_linenumber = null;
            int dest_offset = -1;
            
            string t = "";
            bool insert_mode = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    if (args[i - 1] == "-l")
                        dest_linenumber = new intrange(args[i]);
                    if (args[i - 1] == "-c")
                        Int32.TryParse(args[i], out dest_offset);
                }
                if (args[i] == "-i")
                    insert_mode = true;
                t = args[i];
            }
            //Console.WriteLine("linenumbers:" + dest_linenumber.ToString() + dest_linenumber.range.ToString());
            System.IO.StreamReader iosin = new System.IO.StreamReader(Console.OpenStandardInput());
            System.IO.StreamWriter iosout = new System.IO.StreamWriter(Console.OpenStandardOutput());
            while (!iosin.EndOfStream)
            {
                string curr_line = iosin.ReadLine();
                //Pad out short or empty lines
                if (String.IsNullOrEmpty(curr_line) || curr_line.Length < dest_offset)
                    curr_line = curr_line + "".PadLeft(dest_offset - curr_line.Length);
                if ( (dest_linenumber.range && current_linenumber >= dest_linenumber.low && current_linenumber <= dest_linenumber.high) ||
                    (current_linenumber == dest_linenumber.low) )
                {
                    if(insert_mode)
                        curr_line = curr_line.Substring(0, dest_offset) + t + curr_line.Substring(dest_offset);
                    else
                    {
                        if (curr_line.Length > dest_offset + t.Length)
                            curr_line = curr_line.Substring(0, dest_offset) + t + curr_line.Substring(dest_offset + t.Length);   // old + new + old
                        else
                            curr_line = curr_line.Substring(0, dest_offset) + t;  // String + new text exceeds length of original
                    }
                }
                iosout.WriteLine(curr_line);
                current_linenumber++;
            }
            iosout.Close();


        }


    }

    public class intrange
    {
        public readonly int low;
        public readonly int high;
        public readonly bool range;
        public intrange(string input)
        {
            string lowchars = "";
            string highchars = "";
            bool flop = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] >= '0' && input[i] <= '9' && !flop)
                    lowchars += input[i];
                if (input[i] >= '0' && input[i] <= '9' && flop)
                    highchars += input[i];
                if (input[i] < '0' || input[i] > '9')
                    flop = true;
            }
            range = true;
            range = range && int.TryParse(lowchars, out low);
            range = range && int.TryParse(highchars, out high); //If no high chars, then range = false
            if(range && low > high) //Only reorder when range present (otherwise low=? high=0)
            {
                int x = low;
                low = high; high = x;
            }
        }
        public override String ToString()
        {
            if(range) 
                return low.ToString() + "-" + high.ToString();
            return low.ToString();
        }

    }
}
