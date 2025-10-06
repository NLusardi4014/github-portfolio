using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TeamSk85GameProject
{
    internal class CopCar
    {
        //declared location and 2D texture
        private Rectangle carLoc;
        private Texture2D carSprite;

        //fields for whether the cop is active and the distance, used in other calculations
        private bool isActive = false;
        private double copDistance;

        private int copSpeed = 2;
        
        /// <summary>
        /// A property for setting and returning whether the cop is active or not
        /// </summary>
        public bool IsActive { get { return isActive; } set { isActive = value; } }

        /// <summary>
        /// A property for the cop's distance for calculations
        /// </summary>
        public double CopDistance { get { return copDistance; } set { copDistance = value; } }

        /// <summary>
        /// Property that provides a referance for Game1
        /// </summary>
        public Rectangle CarLoc
        {
            get { return carLoc; }
        }

        /// <summary>
        /// Constructor which initializes the sprite and car's location
        /// </summary>
        /// <param name="carSprite">The sprite to draw</param>
        /// <param name="carLoc">The location the car should start at</param>
        public CopCar(Texture2D carSprite, Rectangle carLoc)
        {
            this.carSprite = carSprite;
            this.carLoc = carLoc;   
        }

        /// <summary>
        /// Method for initializing the cop's distance when the cop is activated
        /// </summary>
        /// <param name="distance">The distance of the player, used when initializing the cop</param>
        public void Initialize(double distance)
        {
            copDistance = distance - 10;
            IsActive = true;
        }

        /// <summary>
        /// A custom update method to be called in Game1
        /// </summary>
        /// <param name="gameTime">Needed for the distance update</param>
        /// <param name="distance">The player's distance</param>
        /// <param name="speed">the speed the game is running at</param>
        public void Update(GameTime gameTime, double distance, float speed)
        {
            CopDistance += (gameTime.ElapsedGameTime.TotalSeconds * 1.35);

            //if the game's speed is greater than 1.2, the cop will "fall behind"
            if (speed > 1.35)
            {
                carLoc.X -= (int)(copSpeed * 1.5);
            }
            else if ((distance - CopDistance) < 12)//Otherwse the cop will "catch up"
            {
                carLoc.X += copSpeed;
            }
            
            if ((distance - CopDistance) > 30)
            {
                CopDistance = CopDistance + 20;
            }

            if (carLoc.X < -160) { carLoc.X = -160; }
        }

        /// <summary>
        /// A custom draw method to be called in Game1
        /// </summary>
        /// <param name="spriteBatch">The spritebatch for the game</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(carSprite, CarLoc, Color.White);
        }

        /// <summary>
        /// Check's if the cop and player should be intersecting
        /// </summary>
        /// <param name="distance">The distance of the player</param>
        /// <param name="playerLoc">The player's location</param>
        /// <returns></returns>
        public bool CheckCollision(double distance, Rectangle playerLoc)
        {
            //returns true if either the cop's distance is greater than the player's, 
            //or if the cop intersects the player. Returns false otherwise. 
            if (CopDistance > distance || carLoc.Intersects(playerLoc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// method to restart when returning to menu
        /// </summary>
        /// <param name="oldLoc">the default position of the cop</param>
        public void CopReset(Rectangle oldLoc)
        {
            copDistance = 0;
            carLoc = oldLoc;
        }
    }
}
