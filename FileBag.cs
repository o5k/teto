using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Teto {
    /// <summary>
    /// A bag-sorted collection of files.
    /// </summary>
    public class FileBag {
        /// <summary>
        /// The directory path of this bag.
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The files queued for use.
        /// </summary>
        private List<string> futureBag = new List<string>();
        /// <summary>
        /// The files that have been used already.
        /// </summary>
        private List<string> pastBag = new List<string>();
        /// <summary>
        /// The instance's randomizer.
        /// </summary>
        private Random rng = new Random();

        /// <summary>
        /// Gets the count of items remaining in this bag.
        /// </summary>
        public int Count {
            get {
                return futureBag.Count;
            }
        }
        /// <summary>
        /// Gets the total count of items in the bag.
        /// </summary>
        public int TotalCount {
            get {
                return futureBag.Count + pastBag.Count;
            }
        }

        /// <summary>
        /// Construct a file bag.
        /// </summary>
        /// <param name="path">The directory path of the bag to be created.</param>
        public FileBag(string path) {
            Path = path;
            Update(true);
        }

        /// <summary>
        /// Shuffles the bag in place.
        /// </summary>
        public void Shuffle() {
            for (var i = 0; i < futureBag.Count - 1; i++) {
                int j = rng.Next(i, futureBag.Count);
                string t = futureBag[i];
                futureBag[i] = futureBag[j];
                futureBag[j] = t;
            }
        }

        /// <summary>
        /// Reloads the bag's file listing.
        /// </summary>
        /// <param name="reset">Whether to forget progress (will reshuffle)</param>
        public void Update(bool reset) {
            string[] files = Directory.GetFiles(Path);

            if (reset) {
                futureBag = new List<string>();
                pastBag = new List<string>();

                futureBag.AddRange(files);
                Shuffle();
            } else {
                List<string> filesThatShouldExist = new List<string>();
                filesThatShouldExist.AddRange(futureBag);
                filesThatShouldExist.AddRange(pastBag);

                foreach (string file in files) {
                    if (filesThatShouldExist.Contains(file)) {
                        // file has not changed
                        filesThatShouldExist.Remove(file);
                    } else {
                        // new file
                        futureBag.Insert(rng.Next(0, futureBag.Count), file);
                        The.Low($"Added new file { System.IO.Path.GetFileName(file) } to bag (now { futureBag.Count } files).");
                    }
                }

                foreach (string file in filesThatShouldExist) {
                    // file was deleted
                    futureBag.Remove(file);
                    pastBag.Remove(file);
                    The.Low($"Removed deleted file { System.IO.Path.GetFileName(file) } from bag (now { futureBag.Count } files).");
                }
            }
        }

        /// <summary>
        /// Grab a file out of this bag.
        /// </summary>
        /// <returns>The file grabbed.</returns>
        public string Take() {
            Update(futureBag.Count == 0);

            if (futureBag.Count == 0) {
                throw new IOException("The bag is empty.");
            }

            string taken = futureBag.First();
            futureBag.RemoveAt(0);
            pastBag.Add(taken);

            return taken;
        }
    }
}
