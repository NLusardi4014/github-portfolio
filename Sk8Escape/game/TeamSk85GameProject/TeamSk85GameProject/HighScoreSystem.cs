using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace TeamSk85GameProject
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    internal class HighScoreSystem
    {
        private const string HighScoreFile = "../../../HighScores.txt";
        public static List<(string Name, int Score)> HighScores;

        //the constructor just calls LoadHighScores
        public HighScoreSystem()
        {
            LoadHighScores();
        }

        public static void DrawHighScores(SpriteBatch sb, SpriteFont font)
        {
            Vector2 drawLocation = new Vector2(600, 150);

            foreach ((string name,  int score) in HighScores)
            {
                sb.DrawString(font, string.Format("{0},{1}",name.ToUpper(),score), drawLocation,Color.Maroon);
                drawLocation.Y += 60;
            }
        }
        
        /// <summary>
        /// LoadsHighScores so HighScoreSystem has access to it
        /// </summary>
        public static void LoadHighScores()
        {

            //clears the highscore list
            HighScores = new List<(string, int)>();
            StreamReader highScoreReader = null;
            try
                //then reads the high score file
            {
                highScoreReader = new StreamReader(HighScoreFile);

                string line;
                //line by line
                while((line = highScoreReader.ReadLine()) != null)
                {
                    //and adds the scores to HighScores
                    string[] input = line.Split(',');
                    HighScores.Add((input[0], int.Parse(input[1])));

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            highScoreReader.Close();
        }

        //this is only called when the player score is greater than one of the high scores
        public static void SaveHighScores(string playerName, int playerScore)
        {
            // Add player's score to high scores list
            HighScores.Add((playerName, playerScore));

            // Sort high scores by score (descending)
            HighScores.Sort((a, b) => b.Score.CompareTo(a.Score));

            // make it so only the top 10 save
            while (HighScores.Count > 10)
            {
                HighScores.RemoveAt(HighScores.Count - 1); // Remove the last element
            }

            // Once its trimmed down to just ten, write the highscores file again
            StreamWriter highScoreWriter = new StreamWriter(HighScoreFile);
            foreach ((string Name, int Score) score in HighScores)
            {
                // Writes each high score as name,score to the file
                highScoreWriter.WriteLine("{0},{1}",score.Name,score.Score);
            }
            highScoreWriter.Close();

        }
        /// <summary>
        /// returns the lowest score as an int
        /// </summary>
        /// <returns></returns>
        public int LowestScore()
        {
            int lowest = HighScores.Last().Score;
            return lowest;
        }
    }
}
