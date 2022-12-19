using Npgsql;
using NpgsqlTypes;
using System.Collections;

public class WriteToDatabase
{
    public static void Main() 
    {
        //string connectionString = "Host=192.168.1.89;Port=5432;Username=betapet;Password=?;Database=betapet;Trust Server Certificate=True";
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=?;Database=postgres;Trust Server Certificate=True";
        string directoryFile = "C:\\users\\adam\\desktop\\betapet-bot-api\\Betapet\\swe_wordlist.txt";   // directory to the file you want to read from

        HashSet<string> words = new HashSet<string>();

        AddToHashSet(words, "C:\\users\\adam\\desktop\\betapet-bot-api\\Betapet\\swe_wordlist.txt");
        AddToHashSet(words, "C:\\users\\adam\\desktop\\betapet-bot-api\\Betapet\\svenska-ord.txt");

        WriteFromFileToDatabase(connectionString, words.ToList());
    }

    private static void AddToHashSet(HashSet<string> set, string filePath)
    {
        string[] rows = File.ReadAllLines(filePath);

        foreach(string row in rows)
        {
            set.Add(row.Trim().ToUpper());
        }
    }

    public static void WriteFromFileToDatabase(string connectionString, List<string> words) 
    {
        string query = "INSERT INTO lexicon(word, letter_value, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, r, s, t, u, v, w, x, y, z, a1, a2, o2) VALUES (@word, @letter_value, @a, @b, @c, @d, @e, @f, @g, @h, @i, @j, @k, @l, @m, @n, @o, @p, @r, @s, @t, @u, @v, @w, @x, @y, @z, @a1, @a2, @o2)";

        foreach (string word in words)
        {
            if (string.IsNullOrEmpty(word.Trim()))
                continue;

            bool shouldContinue = false;

            foreach(char c in word.ToUpper().Trim())
            {
                if(!characters.Contains(c))
                {
                    shouldContinue = true;
                    break;
                }
            }

            if (shouldContinue)
                continue;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand commnd = new NpgsqlCommand(query, connection))
            {
                connection.Open();

                commnd.Parameters.Add("@word", NpgsqlDbType.Varchar).Value = word.ToUpper().Trim();
                commnd.Parameters.Add("@letter_value", NpgsqlDbType.Integer).Value = GetLetterValue(word.ToUpper());

                for (int i = 0; i < characters.Length - 3; i++)
                {
                    commnd.Parameters.Add("@" + characters[i], NpgsqlDbType.Smallint).Value = GetLetterCount(word.ToUpper().Trim(), characters[i]);
                }

                commnd.Parameters.Add("@a1", NpgsqlDbType.Smallint).Value = GetLetterCount(word.ToUpper().Trim(), 'Å');
                commnd.Parameters.Add("@a2", NpgsqlDbType.Smallint).Value = GetLetterCount(word.ToUpper().Trim(), 'Ä');
                commnd.Parameters.Add("@o2", NpgsqlDbType.Smallint).Value = GetLetterCount(word.ToUpper().Trim(), 'Ö');

                commnd.ExecuteNonQuery();
            }
        }
    }

    private const string characters = "ABCDEFGHIJKLMNOPRSTUVWXYZÅÄÖ";

    public static int GetLetterValue(string word)
    {
        BitArray bitArray = new BitArray(32);

        for (int i = 0; i < characters.Length; i++)
        {
            bool exists = false;

            for (int c = 0; c < word.Length; c++)
            {
                if (word[c] == characters[i])
                {
                    exists = true;
                    break;
                }
            }

            bitArray[i] = exists;
        }

        int[] array = new int[1];
        bitArray.CopyTo(array, 0);
        return array[0];
    }

    public static short GetLetterCount(string word, char c)
    {
        short count = 0;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == c)
                count++;
        }

        return count;
    }
}