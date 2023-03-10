using System.Text.RegularExpressions;

namespace MDR_Harvester.Extensions;

public class IECHelpers
{
    public List<Criterion>? GetNumberedCriteria(string sid, string input_string, string type)
    {
        if (string.IsNullOrEmpty(input_string))
        {
            return null;
        }
        else
        {
            // Establish criteria list to receive results,
            // and set up criterion type codes to be used.

            List<Criterion> cr = new();

            int single_crit = type == "inclusion" ? 1 : 2;
            int post_crit = single_crit + 200;
            int grp_hdr = single_crit + 300;
            int no_sep = single_crit + 1000;

            string single_type = type + " criterion";
            string post_crit_type = type + " criteria supplementary statement";
            string grp_hdr_type = type + " criteria group heading";
            string no_sep_type = type + " with no separator";

            string[] lines = input_string.Split('\n');
            if (lines.Length == 1)
            {
                // no carriage return separators in the input string...
                cr.Add(new Criterion(1, "All", 0, 1, no_sep, no_sep_type, input_string));
                return cr;
            }
            else
            {
                Regex ressh = new Regex(@"^\d{1,2}\.\d{1,2}\.\d{1,2}\.");
                Regex resh = new Regex(@"^\d{1,2}\.\d{1,2}\.");
                Regex resh1 = new Regex(@"^\d{1,2}\.\d{1,2} ");
                Regex reha = new Regex(@"^[a-z]{1}\.");
                Regex rehab = new Regex(@"^[a-z]{1}\)");
                Regex renha = new Regex(@"^\d{1,2}[a-z]{1} ");
                Regex retab1 = new Regex(@"^-\t");
                Regex retab2 = new Regex(@"^\d{1,2}\t");
                Regex retab3 = new Regex(@"^\uF0A7\t");
                Regex retab4 = new Regex(@"^\*\t");
                Regex retab5 = new Regex(@"^[a-z]\.\t");
                Regex rebrnum = new Regex(@"^\(\d{1,2}\)");
                Regex resbrnum = new Regex(@"^\d{1,2}\)");
                Regex rebrnumdot = new Regex(@"^\d{1,2}\)\.");
                Regex resqbrnum = new Regex(@"^\[\d{1,2}\]");
                Regex rebull = new Regex(@"^[\u2022,\u2023,\u25E6,\u2043,\u2219]");
                Regex rebull1 = new Regex(@"^[\u2212,\u2666,\u00B7,\uF0B7]");
                Regex reso = new Regex(@"^o ");
                Regex reslat = new Regex(@"^x{0,3}(ix|iv|v?i{0,3})\)");
                Regex redash = new Regex(@"^-");
                Regex restar = new Regex(@"^\*");                
                Regex recrit = new Regex(@"^\d{1,2}\. ");
                Regex recrit1 = new Regex(@"^\d{1,2}\.");

                int level = 0;
                string oldHdrName = "none";
                List<Level> levels = new(){new Level("none", 0)}; 
               
                for (int i = 0; i < lines.Length; i++)
                {
                    string this_line = lines[i].TrimPlus()!;
                    if (!string.IsNullOrEmpty(this_line)
                        && !this_line.Contains(new string('_', 4)))
                    {
                        this_line = this_line.Replace("..", ".");
                        this_line = this_line.Replace(",.", ".");
                        this_line = this_line.Replace("\n\n", "\n");

                        string hdrName;
                        string regex_pattern;
                        if (recrit.IsMatch(this_line)) //  Number period and space  1. , 2. 
                        {
                            hdrName = "recrit";
                            regex_pattern = @"^\d{1,2}\. ";
                        }
                        else if (resh.IsMatch(this_line)) // Numeric Sub-heading. N.n.
                        {
                            hdrName = "resh";
                            regex_pattern = @"^\d{1,2}\.\d{1,2}\.";
                        }
                        else if (resh1.IsMatch(this_line)) // Numeric Sub-heading (without final period) N.n
                        {
                            hdrName = "resh1";
                            regex_pattern = @"^\d{1,2}\.\d{1,2} ";
                        }
                        else if (ressh.IsMatch(this_line)) // Numeric Sub-sub-heading. N.n.n.
                        {
                            hdrName = "ressh";
                            regex_pattern = @"^\d{1,2}\.\d{1,2}\.\d{1,2}\.";
                        }
                        else if (reha.IsMatch(this_line)) // Alpha heading. a., b.
                        {
                            hdrName = "reha";
                            regex_pattern = @"^[a-z]{1}\.";
                        }
                        else if (rehab.IsMatch(this_line)) // Alpha heading. a), b)
                        {
                            hdrName = "rehab";
                            regex_pattern = @"^[a-z]{1}\)";
                        }
                        else if (renha.IsMatch(this_line)) // Number plus letter - Na, Nb
                        {
                            hdrName = "renha";
                            regex_pattern = @"^\d{1,2}[a-z]{1} ";
                        }
                        else if (retab1.IsMatch(this_line)) // Hyphen followed by tab, -\t, -\t 
                        {
                            hdrName = "retab1";
                            regex_pattern = @"^-\t";
                        }
                        else if (retab2.IsMatch(this_line)) // Number followed by tab, -\1, -\2 
                        {
                            hdrName = "retab2";
                            regex_pattern = @"^\d{1,2}\t"; 
                        }
                        else if (retab3.IsMatch(this_line)) // Unknown character followed by tab
                        {
                            hdrName = "retab3";
                            regex_pattern = @"^\uF0A7\t";
                        }
                        else if (retab4.IsMatch(this_line)) // Asterisk followed by tab
                        {
                            hdrName = "retab4";
                            regex_pattern = @"^\*\t";
                        }
                        else if (retab5.IsMatch(this_line)) // Alpha-period followed by tab   a.\t, b.\t
                        {
                            hdrName = "retab5";
                            regex_pattern = @"^[a-z]\.\t";
                        }
                        else if (rebrnum.IsMatch(this_line)) // Bracketed numbers (1), (2)
                        {
                            hdrName = "rebrnum";
                            regex_pattern = @"^\(\d{1,2}\)";
                        }
                        else if (restar.IsMatch(this_line)) //  Asterisk only
                        {
                            hdrName = "restar";
                            regex_pattern = @"^\*";
                        }
                        else if (resbrnum.IsMatch(this_line)) // Alpha-period followed by tab   a.\t, b.\t
                        {
                            hdrName = "resbrnum";
                            regex_pattern = @"^\d{1,2}\)";
                        }
                        else if (rebrnumdot.IsMatch(this_line)) // Bracketed numbers (1), (2)
                        {
                            hdrName = "rebrnumdot";
                            regex_pattern = @"^\d{1,2}\)\.";
                        }
                        else if (resqbrnum.IsMatch(this_line)) //  Asterisk only
                        {
                            hdrName = "resqbrnum";
                            regex_pattern = @"^\[\d{1,2}\]";
                        }
                        else if (rebull.IsMatch(this_line)) // various bullets
                        {
                            hdrName = "rebull";
                            regex_pattern = @"^[\u2022,\u2023,\u25E6,\u2043,\u2219]";
                        }
                        else if (rebull1.IsMatch(this_line)) // various bullets
                        {
                            hdrName = "rebull1";
                            regex_pattern = @"^[\u2212,\u2666,\u00B7,\uF0B7]";
                        }
                        else if (reso.IsMatch(this_line)) // various bullets
                        {
                            hdrName = "reso";
                            regex_pattern = @"^o ";
                        }
                        else if (reslat.IsMatch(this_line)) // various bullets
                        {
                            hdrName = "reslat";
                            regex_pattern = @"^x{0,3}(ix|iv|v?i{0,3})\)";
                        }
                        else if (redash.IsMatch(this_line)) //  Asterisk only
                        {
                            hdrName = "redash";
                            regex_pattern = @"^-";
                        }
                        else if (recrit1.IsMatch(this_line)) //  Number period only - can (rarely) give false positives
                        {
                            hdrName = "recrit1";
                            regex_pattern = @"^\d{1,2}\.";
                        }
                        else
                        {
                            hdrName = "none";
                            regex_pattern = @"";
                        }
                        
                       
                        if (hdrName != "none")
                        { 
                            if (hdrName != oldHdrName)
                            {
                                level = GetLevel(hdrName, levels);
                            }
                            levels[level].levelNum++;

                            string leader = Regex.Match(this_line, regex_pattern).Value;
                            string clipped_line = Regex.Replace(this_line, regex_pattern, string.Empty).Trim();
                            cr.Add(new Criterion(i + 1, leader, level, levels[level].levelNum,
                                single_crit, single_type, clipped_line));
                        }
                        else
                        {
                            if (i == lines.Length - 1)
                            {
                                cr.Add(new Criterion(i + 1, "Spp", level, levels[level].levelNum, post_crit,
                                    post_crit_type, this_line));
                            }
                            else
                            {
                                cr.Add(new Criterion(i + 1, "Hdr", level, levels[level].levelNum, grp_hdr, 
                                    grp_hdr_type, this_line));
                            }
                        }
                        
                        oldHdrName = hdrName;
                    }
                }
                
                // Repair some of the more obvious mis-interpretations
                // Work backwards and re-aggregate lines split with spurious \n

                if (cr.Count == 2 && cr[0].CritTypeId == grp_hdr
                                  && cr[1].CritTypeId == post_crit)
                {
                    // More likely that the second is a criterion after the heading
                    // rather than a 'supplement' statement.

                    cr[1].CritTypeId = single_crit;
                    cr[1].CritType = single_type;
                    cr[1].Leader = "(1)";
                }

                List<Criterion> cr2 = new();

                for (int i = cr.Count - 1; i >= 0; i--)
                {
                    bool transfer_crit = true;
                    string? thisText = cr[i].CritText;
                    if (!string.IsNullOrEmpty(thisText))
                    {
                        if (cr[i].CritTypeId == grp_hdr 
                            && i < cr.Count - 1 && i > 0
                            && !thisText.EndsWith(':'))
                        {
                            // Does the following entry have an indentation level greater than the header? 
                            // if not it is probably not a 'true' header. Add it to the preceding entry...
                            // (N.B. Initial cr[0] is not checked, nor is the last cr entry).

                            if (cr[i].IndentLevel >= cr[i + 1].IndentLevel)
                            {
                                // Almost certainly a spurious \n in the
                                // original string rather than a genuine header.

                                cr[i - 1].CritText += " " + thisText;
                                cr[i - 1].CritText = cr[i - 1].CritText?.Replace("  ", " ");
                                transfer_crit = false;
                            }
                        }

                        if (cr[i].CritTypeId == post_crit && !thisText.EndsWith(':')
                             && !thisText.StartsWith('*')
                             && !thisText.ToLower().StartsWith("note")
                             && !thisText.ToLower().StartsWith("for further details")
                             && !thisText.ToLower().StartsWith("for more information"))
                        {
                            // Almost always is a spurious supplement.
                            // Whether should be joined depends on whether there is an initial
                            // lower case or upper case letter... 

                            char init = cr[i].CritText![0];
                            if (char.ToLower(init) == init)
                            {
                                cr[i - 1].CritText += " " + thisText;
                                cr[i - 1].CritText = cr[i - 1].CritText?.Replace("  ", " ");
                                transfer_crit = false;
                            }
                            else
                            {
                                cr[i].CritTypeId = single_crit;
                                cr[i].CritType = single_type;
                                cr[i].IndentLevel = cr[i - 1].IndentLevel;
                                cr[i].LevelSeqNum = cr[i - 1].LevelSeqNum + 1;
                            }
                        }

                        if (transfer_crit)
                        {
                            cr2.Add(cr[i]);
                        }
                    }
                }

                return cr2.OrderBy(c => c.SeqNum).ToList();
            }
        }
    }

    private int GetLevel(string hdr_name, List<Level> levels)
    {
        if (levels.Count == 1)
        {
            levels.Add(new Level(hdr_name, 0));
            return 1;
        }
        else
        {
            // See if the level header has been used - if so
            // return level, if not add and return new level

            for (int i = 0; i < levels.Count; i++)
            {
                if (hdr_name == levels[i].levelName)
                {
                    return i;
                }
            }
            levels.Add(new Level(hdr_name, 0));
            return levels.Count - 1;
        }
    }
    

    public record Level
    {
        public Level(string _levelName, int _levelNum)
        {
            levelName = _levelName;
            levelNum = _levelNum;
        }
        public string levelName { get; set; }
        public int levelNum { get; set; }

    }
}
