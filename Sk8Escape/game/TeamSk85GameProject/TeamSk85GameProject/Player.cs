using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TeamSk85GameProject
{
    enum PlayerState
    {
        Standing,        
        JumpUp,
        SlideDown,
        Grind,
        GetHit
    }
    internal class Player
    {
        //Varibles and Fields
        private Rectangle playerLoc;
        private PlayerState state;

        private Texture2D spriteSheet;
        private Texture2D spriteSheetAfterImages;

        private KeyboardState prevKBState;
        private int floor;
        private double playerDistance;
        private double playerScore;

        int frame;
        double timeCounter;
        double fps;
        double timePerFrame;
        double ySpeed;
        bool grounded = true;

        //Constants
        const double Gravity = 0.75;
        const int WalkFrameCount = 3;
        const int PlayerRectOffsetY = 0;   // How far down in the image are the frames?
        const int PlayerRectHeight = 128;     // The height of a single frame
        const int PlayerRectWidth = 128;      // The width of a single frame


        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Property that provides a referance for Game1
        /// </summary>
        public Rectangle PlayerLoc 
        { 
            get { return playerLoc; } 
        }

        public double PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }

        public double PlayerDistance
        {
            get { return playerDistance; }
            set { playerDistance = value; }
        }
        /// <summary>
        /// Constructor for player
        /// </summary>
        /// <param name="spriteSheet">spritesheet for player</param>
        /// <param name="playerLoc">player location</param>
        /// <param name="startingState">state the character starts</param>
        /// <param name="floor">boolean to check if player is with contact with the floor</param>
        public Player(Texture2D spriteSheet, Texture2D spriteSheetAfterImages,Rectangle playerLoc, PlayerState startingState, int floor)
        {
            this.spriteSheet = spriteSheet;
            this.spriteSheetAfterImages = spriteSheetAfterImages;
            this.playerLoc = playerLoc;
            this.state = startingState;
            this.floor = floor;
            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
        }

        /// <summary>
        /// update animation mehod
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }

        /// <summary>
        /// Draw method for each state
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, float speed)
        {

            switch (state)
            {
                case PlayerState.Standing:
                    //draw after images for Standing
                    if (speed > 1.2)
                    {
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.2f, new Vector2(playerLoc.X + 80 - playerLoc.X / 5, playerLoc.Y), spriteSheetAfterImages);
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.3f, new Vector2(playerLoc.X + 80 - playerLoc.X / 6, playerLoc.Y), spriteSheetAfterImages);
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.4f, new Vector2(playerLoc.X + 80 - playerLoc.X / 7, playerLoc.Y), spriteSheetAfterImages);
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 8, playerLoc.Y), spriteSheetAfterImages);
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 9, playerLoc.Y), spriteSheetAfterImages);
                        DrawStanding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 10, playerLoc.Y), spriteSheetAfterImages);
                    }
                    DrawStanding(SpriteEffects.None, spriteBatch, Color.White, new Vector2(playerLoc.X,playerLoc.Y), spriteSheet);
                    break;               
                case PlayerState.JumpUp:
                    //draw after images for Jumping
                    if (speed > 1.2)
                    {
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.2f, new Vector2(playerLoc.X + 80 - playerLoc.X / 5, playerLoc.Y), spriteSheetAfterImages);
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.3f, new Vector2(playerLoc.X + 80 - playerLoc.X / 6, playerLoc.Y), spriteSheetAfterImages);
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.4f, new Vector2(playerLoc.X + 80 - playerLoc.X / 7, playerLoc.Y), spriteSheetAfterImages);
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 8, playerLoc.Y), spriteSheetAfterImages);
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 9, playerLoc.Y), spriteSheetAfterImages);
                        DrawJumping(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 10, playerLoc.Y), spriteSheetAfterImages);
                    }
                    DrawJumping(SpriteEffects.None, spriteBatch, Color.White, new Vector2(playerLoc.X, playerLoc.Y), spriteSheet);
                    break;
                case PlayerState.SlideDown:
                    //draw after images for Sliding
                    if (speed > 1.2)
                    {
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.2f, new Vector2(playerLoc.X + 80 - playerLoc.X / 5, playerLoc.Y), spriteSheetAfterImages);
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.3f, new Vector2(playerLoc.X + 80 - playerLoc.X / 6, playerLoc.Y), spriteSheetAfterImages);
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.4f, new Vector2(playerLoc.X + 80 - playerLoc.X / 7, playerLoc.Y), spriteSheetAfterImages);
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 8, playerLoc.Y), spriteSheetAfterImages);
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 9, playerLoc.Y), spriteSheetAfterImages);
                        DrawSliding(SpriteEffects.None, spriteBatch, Color.Yellow * 0.5f, new Vector2(playerLoc.X + 80 - playerLoc.X / 10, playerLoc.Y), spriteSheetAfterImages);
                    }
                    DrawSliding(SpriteEffects.None, spriteBatch, Color.White, new Vector2(playerLoc.X, playerLoc.Y), spriteSheet);
                    break;
                case PlayerState.Grind:
                    DrawGriding(SpriteEffects.None, spriteBatch, Color.White, new Vector2(playerLoc.X, playerLoc.Y), spriteSheet);
                    break;
                case PlayerState.GetHit:
                    break;
            }
        }

        /// <summary>
        /// Update method for each state
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, float speed)
        {
            //Kb state
            KeyboardState kbState = Keyboard.GetState();

            //Player location change for Y
            playerLoc.Y += (int)ySpeed;

            //player is not touching floor
            if (playerLoc.Y > floor)
            {
                playerLoc.Y = floor;
                grounded = true;
            }

            switch (state)
            {
                //Case Standing
                case PlayerState.Standing:
                    ySpeed = 0;

                    //If the player Presses Left Key arrow or A
                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {

                        //If the player Presses Up Key arrow or W or Space Bar
                        playerLoc.X -= 12;
                        if ((SingleKeyPress(Keys.Space, kbState) ||
                        SingleKeyPress(Keys.Up, kbState) ||
                        SingleKeyPress(Keys.W, kbState)) && grounded)
                        {
                            ySpeed = -16;
                            grounded = false;
                            state = PlayerState.JumpUp;
                        }

                        //If the player Presses Down Key arrow or S
                        else if (kbState.IsKeyDown(Keys.Down) || kbState.IsKeyDown(Keys.S))
                        {
                            state = PlayerState.SlideDown;
                        }
                    }

                    //If the player Presses Right Key arrow or D
                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        playerLoc.X += 12;

                        //If the player Presses Up Key arrow or W or Space Bar
                        if ((SingleKeyPress(Keys.Space, kbState) ||
                        SingleKeyPress(Keys.Up, kbState) ||
                        SingleKeyPress(Keys.W, kbState)) && grounded)
                        {
                            ySpeed = -16;
                            grounded = false;
                            state = PlayerState.JumpUp;
                        }

                        //If the player Presses Down Key arrow or S
                        else if (kbState.IsKeyDown(Keys.Down) || kbState.IsKeyDown(Keys.S))
                        {
                            state = PlayerState.SlideDown;
                        }
                    }

                    //If the player Presses Down Key arrow or S
                    else if (kbState.IsKeyDown(Keys.Down) || kbState.IsKeyDown(Keys.S))
                    {
                        state = PlayerState.SlideDown;
                    }

                    //If the player Presses Up Key arrow or W or Space Bar
                    else if ((SingleKeyPress(Keys.Space, kbState) || 
                        SingleKeyPress(Keys.Up, kbState) || 
                        SingleKeyPress(Keys.W, kbState)) && grounded)
                    {
                        ySpeed = -16;
                        grounded = false;
                        state = PlayerState.JumpUp;
                    }
                    break;

                
                //Case Jump Up
                case PlayerState.JumpUp:                   
                    ySpeed += Gravity;

                    if (grounded)
                    {
                        state = PlayerState.Standing;
                    }

                    //If the player Presses Right Key arrow or D
                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        playerLoc.X += 8;
                    }

                    //If the player Presses Left Key arrow or A
                    if (kbState.IsKeyDown(Keys.Left)|| kbState.IsKeyDown(Keys.A))
                    {
                        playerLoc.X -= 8;
                    }
                    break;

                //Case Slide Down
                case PlayerState.SlideDown:                    
                    if (kbState.IsKeyUp(Keys.Down) && kbState.IsKeyUp(Keys.S))
                    {
                        state = PlayerState.Standing;
                    }
                    //If the player Presses Up Key arrow or W or Space Bar
                    else if ((SingleKeyPress(Keys.Space, kbState) || 
                        (SingleKeyPress(Keys.Up, kbState)) || 
                        SingleKeyPress(Keys.W, kbState)) && grounded)
                    {
                        ySpeed = -16;
                        grounded = false;
                        state = PlayerState.JumpUp;
                    }

                    //If the player Presses Right Key arrow or D
                    if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                    {
                        playerLoc.X += 8;
                    }

                    //If the player Presses Left Key arrow or A
                    if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                    {
                        playerLoc.X -= 8;
                    }
                    break;

                //Case Grind
                case PlayerState.Grind:
                    break;

                //Case Get Hit
                case PlayerState.GetHit:
                    break;


            }

            //Limit of movement when it hits the borders
            if (playerLoc.X > 1536 - playerLoc.Width)
            {
                playerLoc.X = 1536 - playerLoc.Width;
            }
            if (playerLoc.X < 0)
            {
                playerLoc.X = 0;
            }
            if (playerLoc.Y > 864 - playerLoc.Height)
            {
                playerLoc.Y = 864 - playerLoc.Height;
            }
            if (playerLoc.Y < 0)
            {
                playerLoc.Y = 0;
            }

            prevKBState = kbState;
        }



        /// <summary>
        /// Method to draw player standing
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch, Color color, Vector2 location, Texture2D spriteSheet)
        {
            spriteBatch.Draw(
                spriteSheet,                            // - The texture to draw
                location, // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    0,                                  //   - This rectangle specifies
                    PlayerRectOffsetY,                   //	   where "inside" the texture
                    PlayerRectWidth,                     //     to get pixels (We don't want to
                    PlayerRectHeight),                   //     draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);
        }

        /// <summary>
        /// Method to draw player sliding
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawSliding(SpriteEffects flipSprite, SpriteBatch spriteBatch, Color color, Vector2 location, Texture2D spriteSheet)
        {
            spriteBatch.Draw(
                spriteSheet,                            // - The texture to draw
                location, // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    2*PlayerRectWidth,                                  //   - This rectangle specifies
                    PlayerRectOffsetY,                   //	   where "inside" the texture
                    PlayerRectWidth,                     //     to get pixels (We don't want to
                    PlayerRectHeight),                   //     draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);
        }

        /// <summary>
        /// Method to draw player jumping
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawJumping(SpriteEffects flipSprite, SpriteBatch spriteBatch, Color color, Vector2 location, Texture2D spriteSheet)
        {
            spriteBatch.Draw(
                spriteSheet,                            // - The texture to draw
                location, // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    PlayerRectWidth,                                  //   - This rectangle specifies
                    PlayerRectOffsetY,                   //	   where "inside" the texture
                    PlayerRectWidth,                     //     to get pixels (We don't want to
                    PlayerRectHeight),                   //     draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);
        }

        /// <summary>
        /// Method to draw player griding
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawGriding(SpriteEffects flipSprite, SpriteBatch spriteBatch, Color color, Vector2 location, Texture2D spriteSheet)
        {
            spriteBatch.Draw(
                spriteSheet,                            // - The texture to draw
                location, // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    3 * PlayerRectWidth,                  //   - This rectangle specifies
                    PlayerRectOffsetY,                   //	   where "inside" the texture
                    PlayerRectWidth,                     //     to get pixels (We don't want to
                    PlayerRectHeight),                   //     draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);
        }

        /// <summary>
        /// Method that checks to see whether a button was pressed. 
        /// Only returns true on the first frame the button was pressed
        /// </summary>
        /// <param name="key"> The specific key being checked </param>
        /// <param name="kbState"> The current keyboard state </param>
        /// <returns> Returns a boolian </returns>
        public bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            return kbState.IsKeyDown(key) && prevKBState.IsKeyUp(key);
        }

        public void UpdateDistance(GameTime gameTime, float speed)
        {
            PlayerDistance += (gameTime.ElapsedGameTime.TotalSeconds * speed);
        }

        public void UpdateScore(GameTime gameTime, float speed)
        {
            //these are all seperate on purpose in case we add the # of obstacles to the score
            //or if the scoring system changes
            int scoreMult = 1 + (int)playerDistance / 100;
            double scoreIncrement = gameTime.ElapsedGameTime.TotalSeconds * speed * speed * 10 * scoreMult;
            PlayerScore += scoreIncrement;
        }

        /// <summary>
        /// Method to reset the player when returning to menu
        /// </summary>
        /// <param name="oldLoc">the default position of the player</param>
        public void PlayerReset(Rectangle oldLoc)
        {
            playerLoc = oldLoc;
            PlayerDistance = 0;
        }
    }
}
