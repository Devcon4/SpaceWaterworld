/* Purpose: Probability Generator. Used to pick randomly from a list based on probability.
 * 
 * Special notes: N/A.
 * 
 * Author: Devyn Cyphers; Devcon.
 */

using System;
using System.Collections.Generic;
/// <summary>
/// Probability Generator. Used to pick randomly from a list based on probability.
/// </summary>
class ProbGen {

    /// <summary>
    /// The main list of Objects and their corresponding probability.
    /// </summary>
    public List<KeyValuePair<Object, float>> thisList = new List<KeyValuePair<Object, float>>();

    // Random used in generating probability.
    private Random r = new Random();

    /// <summary>
    /// M- Probability Generator. Used to pick randomly from a list based on probability.
    /// </summary>
    /// <param name="thisList"> The list of objects and their probability. </param>
    public ProbGen(List<KeyValuePair<Object, float>> thisList_) {
        thisList = thisList_;

        // Catch if probability doesn't add up to 1.
        checkProb();
    }

    /// <summary>
    /// Used to check if the list's probability adds up to 100%.
    /// </summary>
    private void checkProb() {

        float cumulative = 0;

        foreach (KeyValuePair<Object, float> thisPair in thisList) {
            cumulative += thisPair.Value;
        }

        if (cumulative < 1) {
            throw new System.ArgumentException("Probability must we equal to 1.", this.ToString());
        }

    }

    /// <summary>
    /// Returns the next probable Object.
    /// </summary>
    /// <returns>Object</returns>
    public Object next() {
        float test = (float)r.NextDouble();
        float cumulative = 0;
        foreach (KeyValuePair<Object, float> thisPair in thisList) {
            cumulative += thisPair.Value;

            if (test < cumulative) {
                return thisPair.Key;
            }
        }

        return null;
    }

}
