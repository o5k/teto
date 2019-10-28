using System;
using System.IO;
using System.Threading;

namespace Teto {
    class Program {
        static void Main(string[] args) {
            The.DrawLogo();

            #region LoadConfig
            // Load the config
            Config c = new Config("teto.cfg");

            // Wait for config to exist
            if (!c.Exists) {
                The.Warning($"No config file present. I'm making one for you at { Directory.GetCurrentDirectory() }\\teto.cfg, but you'll have to fill in some values.");
                File.WriteAllBytes("teto.cfg", Properties.Resources.teto_cfg);
                c.Load();
            }

            // Wait for missing keys to be populated
            while (true) {
                string[] missingKeys = c.RequireKeys(new string[] {
                    "consumer_key", "consumer_secret", "access_token", "access_secret", "library_folder", "schedule_y", "schedule_mo", "schedule_dow", "schedule_d", "schedule_h", "schedule_m"
                });
                if (missingKeys.Length == 0) {
                    break;
                }

                The.Failure($"The config file is incomplete. Missing keys: '{ string.Join("', '", missingKeys) }'. Add them, then press ENTER...");
                Console.ReadLine();
                c.Load();
            }

            The.Low($"The config file was loaded.");
            #endregion

            // Create a Twitter instance
            Twitter t = new Twitter(new OAuth(c["consumer_key"].Value<string>(), c["consumer_secret"].Value<string>(), c["access_token"].Value<string>(), c["access_secret"].Value<string>()));

            // Create a file bag
            while (!Directory.Exists(c["library_folder"].Value<string>())) {
                The.Failure($"The library folder does not exist. Please create a folder at { Directory.GetCurrentDirectory() }\\{ c["library_folder"].Value<string>() }, then press ENTER...");
                Console.ReadLine();
            }
            FileBag fb = new FileBag(c["library_folder"].Value<string>());
            while (fb.TotalCount == 0) {
                The.Failure($"The library folder is empty. Please fill the folder at { Directory.GetCurrentDirectory() }\\{ c["library_folder"].Value<string>() }, then press ENTER...");
                Console.ReadLine();
                fb.Update(true);
            }
            The.Low($"Found { fb.TotalCount } files; added them to the bag.");

            // Create a schedule
            Schedule s = new Schedule(c["schedule_y"].Value<string>(), c["schedule_mo"].Value<string>(), c["schedule_dow"].Value<string>(), c["schedule_d"].Value<string>(), c["schedule_h"].Value<string>(), c["schedule_m"].Value<string>(), () => {
                The.Low("Schedule triggered.");
                TweetARandomImage(fb, t, c);
            });

            // Ping the schedule once a minute
            int lastMinute = -1;
            while (true) {
                Thread.Sleep(1000);

                if (lastMinute == DateTime.Now.Minute) {
                    continue;
                }
                lastMinute = DateTime.Now.Minute;

                s.Run();
            }
        }

        /// <summary>
        /// The schedule's target.
        /// </summary>
        /// <param name="fb">The file bag to be used.</param>
        /// <param name="t">The Twitter instance to be used.</param>
        /// <param name="c">The config to be used.</param>
        static void TweetARandomImage(FileBag fb, Twitter t, Config c) {
            // Grab a file and set up metadata for it
            string file = fb.Take();
            string filenameFull = Path.GetFileName(file);
            string filename = Path.GetFileNameWithoutExtension(file);
            string pixivId = Path.GetFileNameWithoutExtension(file).Split('_')[0];

            // Construct tweet text
            string tweetText = c["tweet_text"] == null ? "" : c["tweet_text"].Value<string>();
            tweetText = tweetText.Replace("{FILENAME_FULL}", filenameFull);
            tweetText = tweetText.Replace("{FILENAME}", filename);
            tweetText = tweetText.Replace("{PIXIVID}", pixivId);

            // Try to tweet 3 times
            The.Normal($"Tweeting now: \"{ tweetText }\"");

            bool success = false;
            int tries = 3;
            while (tries > 0) {
                tries--;

                try {
                    Webbe.Response res = t.TweetImage(tweetText, file);

                    if (res.Code != 200) {
                        The.Warning($"Tweet failed: received a { res.Code } ... { tries } tries remaining.");
                        Thread.Sleep(5000);
                        continue;
                    }

                    success = true;
                    break;
                } catch(Exception ex) {
                    The.Error($"Exception while tweeting: { ex.Message } ... { tries } tries remaining.");
                    Thread.Sleep(5000);
                    continue;
                }
            }

            if (success) {
                The.Normal("Tweet posted!");
            } else {
                The.Failure("Out of attempts, giving up...");
            }

            The.Low($"Bag status: { fb.Count }/{ fb.TotalCount } remaining until next shuffle");
        }
    }
}
