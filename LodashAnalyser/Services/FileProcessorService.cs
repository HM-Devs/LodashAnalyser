using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FileProcessorService
{
    //ConcurrentDictionary is thread safe made for multiple threads (as we use Parallel.ForEach) also provides access to AddOrUpdate method
    public virtual ConcurrentDictionary<char, int> CountLetterFrequencies(IEnumerable<string> contents)
    {
        var letterCounts = new ConcurrentDictionary<char, int>();

        //Concurrent processing of file contents
        Parallel.ForEach(contents, content =>
        {
            foreach (var letter in content)
            {
                //Consider only the alphabetic value, disregard case sensitivity and ensure abides by ASCII so we only get english characters
                if (char.IsLetter(letter) && letter <= 127)
                {   
                    //Atomically check for key, update value on existence / add new KV pair if not
                    letterCounts.AddOrUpdate(char.ToLower(letter), 1, (key, count) => count + 1);
                }
            }
        });

        return letterCounts;
    }

    //IENumerable provides flexiblity if our implementation changes later by abstracting functionality away
    public virtual IEnumerable<KeyValuePair<char, int>> GetOrderedFrequencies(ConcurrentDictionary<char, int> letterCounts)
    {
        return letterCounts.OrderByDescending(kvp => kvp.Value);
    }
}
