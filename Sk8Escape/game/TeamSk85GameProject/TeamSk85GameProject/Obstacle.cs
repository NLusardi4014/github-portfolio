using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace TeamSk85GameProject
{
    /// <summary>
    /// Enumerator determining what the obstacle's type is, 
    /// used later for a things such as determining weight
    /// </summary>
    enum ObstacleType
    {
        Small,
        WideTallSlide,
        WideShortSlide,
        ThinShortSlide,
        Large,
        ShortRail,
        Rail
    }

    

    internal class Obstacle
    {
        // Type of obstacle
        // Each different obstacle will interact with the player in the same way,
        // but the values will be different. 
        private ObstacleType type;

        //fields
        private Texture2D obsSprite;
        private bool isComplete = false;
        private float sendTime;
        private float consSendTime;
        private float weight;
        private Rectangle position;
        private Rectangle oldPosition;
        private bool isActive = false;
        private bool isCountdown = false;

        //properties
        /// <summary>
        /// Used when the player collides with the obstacle to determine the 
        /// slow strength and the duration
        /// </summary>
        public float Weight
        {
            get { return weight; }
        }

        
        /// <summary>
        /// Returns the position of the obstacle for Game1
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        

        /// <summary>
        /// Keeps track of whether the obstacle is active or not
        /// </summary>
        public bool IsActive { get { return isActive; } set { isActive = value; } }

        /// <summary>
        /// Returns whether the obstacle has been cleared by the player.
        /// This is separate from IsActive as an obstacle can be not active 
        /// but still complete, it's just not on the screen. 
        /// </summary>
        public bool IsComplete { get { return isComplete; } set { isComplete = value; } }

        /// <summary>
        /// The send time of the obstacle, or the countdown. 
        /// </summary>
        public float SendTime
        {
            get { return sendTime; } set { sendTime = value; }
        }

        
        /// <summary>
        /// Boolean for whether the obstacle should be counting down or not. 
        /// </summary>
        public bool IsCountdown { get { return isCountdown; } set {  isCountdown = value; } }   
        
        /// <summary>
        /// Public constructor for creating obstacle objects
        /// </summary>
        /// <param name="type">the type of obstacle, used for weight</param>
        /// <param name="obsSprite">the sprite of the obstacle, used for drawing</param>
        /// <param name="sendTime">The time the obstacle should be sent at the player</param>
        /// <param name="graphicsDevice">graphicsDevice used for SetUp</param>
        public Obstacle(ObstacleType type, Texture2D obsSprite, float sendTime, GraphicsDevice graphicsDevice)
        {
            this.sendTime = sendTime;
            consSendTime = sendTime;
            this.type = type;
            this.obsSprite = obsSprite;
            SetUp(type, graphicsDevice);
        }

        /// <summary>
        /// Update method for the obstacles in Game1
        /// </summary>
        /// <param name="gameTime">the gameTime</param>
        /// <param name="speed">the game's speed variable</param>
        public void Update(GameTime gameTime, float speed)
        {
            int obstacleSpeed = 0;
            //get a speed for the obstacles   
            if (speed > 1.0)
            {
                //int obstacleSpeed = (int)(speed * 12);
                obstacleSpeed = (int)(4 + (speed * speed) * 4);
            }
            else
            {
                //int obstacleSpeed = (int)(speed * 12);
                obstacleSpeed = (int)(4 + (speed * 4));
            }
            

                //and move the obstacle
                position.X -= obstacleSpeed;

                //if the obstacle moves off the side of the screen
                if ((position.X + position.Width) < 0) 
                {
                    //deactivate it
                    isActive = false;
                    isComplete = true;
                }
        }
        
        /// <summary>
        /// Method for setting the obstacles to active
        /// </summary>
        public void Initialize()
        {
            IsActive = true;
        }

        /// <summary>
        /// Method for starting the obstacles to countdown
        /// </summary>
        public void InitializeCountdown()
        {
            isCountdown = true;
        }

        /// <summary>
        /// Draw method for drawing the obstacles in game1
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            //if an obstacle is active
            if (isActive)
            {
                //draw it at the given position
                spriteBatch.Draw(obsSprite, position, Color.White);
            }

        }

        /// <summary>
        /// Reset method for reseting the obstacles when starting a new game
        /// or when all the obstacles had been sent
        /// </summary>
        public void Reset()
        {
            position = oldPosition; 
            sendTime = consSendTime;
            isComplete = false;
            isCountdown = false;
        }

        /// <summary>
        /// Check collision method for Game1
        /// </summary>
        /// <param name="playerLoc">The player's location</param>
        /// <returns></returns>
        public bool CheckCollision(Rectangle playerLoc)
        {
            Rectangle effectivePlayerLoc = new Rectangle(playerLoc.X + 20, playerLoc.Y + 20, playerLoc.Width - 40, playerLoc.Height - 40);
            
            return position.Intersects(effectivePlayerLoc);
        }

        /// <summary>
        /// Method to check if a list of obstacles is fully complete.
        /// This is in the obstacle class but does not actually leverage
        /// any part of it, this is here to get over the accessiblity issues with
        /// putting this method in main. Hence why it's tied to an unactive test obstacle in Game1.
        /// </summary>
        /// <param name="obsList">the list of obstacles to check</param>
        /// <returns>true if all the obstacles in the list are completed, false otherwise</returns>
        public bool CompleteList(List<Obstacle> obsList)
        {
            int completeCount = 0;

            foreach (Obstacle obstacle in obsList)
            {
                if (obstacle.IsComplete)
                {
                    completeCount++;
                }
            }

            if (completeCount == obsList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Method to automaticly set the weight and position of all obstacle 
        /// based on their type
        /// </summary>
        /// <param name="type"> Type of obstacle </param>
        private void SetUp(ObstacleType type, GraphicsDevice graphicsDevice)
        {
            switch (type)
            {
                // Cones and Hedge
                case ObstacleType.Small:
                    weight = 0.3f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 98, 98);
                    oldPosition = position;
                    break;

                // Cars
                case ObstacleType.Large:
                    weight = 0.85f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 294, 98);
                    oldPosition = position;
                    break;

                // The Shorter Rail
                case ObstacleType.ShortRail:
                    weight = 0.3f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 128, 64);
                    oldPosition = position;
                    break;

                // The Taller Rail Rail
                case ObstacleType.Rail:
                    weight = 0.45f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 128, 128);
                    oldPosition = position;
                    break;

                // One Arrow Down Sign
                case ObstacleType.ThinShortSlide:
                    weight = 0.45f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 354, 128, 354);
                    oldPosition = position;
                    break;

                // Three Arrow Down Sign
                case ObstacleType.WideShortSlide:
                    weight = 0.75f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 354, 256, 354);
                    oldPosition = position;
                    break;

                // Tunnel Sign
                case ObstacleType.WideTallSlide:
                    weight = 0.65f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 256, 86);
                    oldPosition = position;
                    break;

                // In case the type doesn't match anything
                default:
                    weight = 0.45f;
                    position = new Rectangle(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height - 226, 128, 128);
                    break;
            }

        }

    }
}
