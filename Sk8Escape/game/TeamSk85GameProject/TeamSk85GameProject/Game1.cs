using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using System;
using System.Diagnostics;

namespace TeamSk85GameProject
{
    enum GameState
    {
        Menu,
        Game,
        Pause,
        ControlsMenu,
        GameOver,
        HighScoreScreen,
        SaveScore
    }

    public class Game1 : Game
    {
        
        private SpriteBatch _spriteBatch;
        private SpriteFont baseFont;
        private SpriteFont scoreFont;
        private GraphicsDeviceManager _graphics;

        // Player Details
        private Player player;
        private Rectangle playerLoc;
        private string playerName = "";

        private CopCar cop;
        private Rectangle copLoc;

        // Temporary obstacle stuff
        private Obstacle tempObstacle;
        private Rectangle obsLoc;

        // Slow down delays list
        private List<float[]> delays = new List<float[]>();

        // Speed Coefficient
        private float speed;

        // Game State
        private GameState gameState;

        // Previous Keyboard State
        private KeyboardState prevKBState;

        //Button List
        private List<Button> buttons = new List<Button>();
        private SpriteFont font;

        //Random variable
        Random rng = new Random();

        //Textures
        private Texture2D assetSheet128x128;
        private Texture2D backgroundSpriteSheet;
        private Texture2D backgroundSpriteSheetEvening;
        private Texture2D backgroundSpriteSheetNight;
        private Texture2D tunnelSprite;
        private Texture2D mainMenu;
        private Texture2D gameOver;
        private Texture2D pauseMenu;
        private Texture2D controlsMenu;
        private Texture2D gameUI;

        //Scrolling
        private int[] backgroundScrollingPositions = {0,0,0,0,-6000};
        private Dictionary<string, Texture2D> obsTextureDict = new Dictionary<string, Texture2D>();
        private float timeOfDayTimer = 0;

        //ObstaclesList
        private List<List<Obstacle>> levelQueues = new List<List<Obstacle>>();

        // current list of obstacles
        int currentObsList = 0;

        //high Score system
        List<Keys> letterKeys = new List<Keys>();
        HighScoreSystem highScoreSystem = new HighScoreSystem();

        public float Speed
        {
            get
            {
                return speed;
            }
        }


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1536;
            _graphics.PreferredBackBufferHeight = 864;
            _graphics.ApplyChanges();
            base.Initialize();
        
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D spriteSheet = Content.Load<Texture2D>("skaterSpriteSheet4X");
            Texture2D spriteSheetAfterImages = Content.Load<Texture2D>("skaterAfterImagesSpriteSheet4X");
            Texture2D copSprite = Content.Load<Texture2D>("copcar");
            Texture2D trafficCone = Content.Load<Texture2D>("trafficCone");
            Texture2D slideUnder = Content.Load<Texture2D>("slideUnder0");
            Texture2D slideUnderSecond = Content.Load<Texture2D>("slideUnder1");
            Texture2D bush = Content.Load<Texture2D>("bush");
            Texture2D car = Content.Load<Texture2D>("car0");
            Texture2D carSecond = Content.Load<Texture2D>("car1");

            
            obsTextureDict["CopSprite"] = copSprite;
            obsTextureDict["TrafficCone"] = trafficCone;
            obsTextureDict["SlideUnder1"] = slideUnder;
            obsTextureDict["SlideUnder2"] = slideUnderSecond;
            obsTextureDict["Bush"] = bush;
            obsTextureDict["Car1"] = car;
            obsTextureDict["Car2"] = carSecond;

            //highscore
            for (Keys key = Keys.A; key <= Keys.Z; key++)
            {
                letterKeys.Add(key);
            }

            //for scrolling testing
            assetSheet128x128 = Content.Load<Texture2D>("assetSheet32x32[4X]");

            baseFont = Content.Load<SpriteFont>("BaseFont");
            scoreFont = Content.Load<SpriteFont>("scoreFont");

            backgroundSpriteSheet = Content.Load<Texture2D>("backgroundSpriteSheet4X");
            backgroundSpriteSheetEvening = Content.Load<Texture2D>("backgroundSpriteSheetEvening4X");
            backgroundSpriteSheetNight = Content.Load<Texture2D>("backgroundSpriteSheetNight4X");
            tunnelSprite = Content.Load<Texture2D>("tunnel[4X]");
            mainMenu = Content.Load<Texture2D>("MainMenuOptions");
            gameOver = Content.Load<Texture2D>("GameOverScreen");
            pauseMenu = Content.Load<Texture2D>("PauseMenu");
            controlsMenu = Content.Load<Texture2D>("ControlsMenu");
            gameUI = Content.Load<Texture2D>("HUD4X");

            // Initiate Player
            playerLoc = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 128, GraphicsDevice.Viewport.Height - 256, 128, 128);
            player = new Player(spriteSheet, spriteSheetAfterImages, playerLoc, PlayerState.Standing, GraphicsDevice.Viewport.Height - 256);

            //temporary object for checking if the list is complete (small cop car)
            obsLoc = new Rectangle(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height - 226, trafficCone.Width-30, trafficCone.Height-30);
            tempObstacle = new Obstacle(ObstacleType.Small,copSprite, 0, GraphicsDevice);
         
            copLoc = new Rectangle(0, GraphicsDevice.Viewport.Height - 344, (int)(copSprite.Width), (int)(copSprite.Height));
            cop = new CopCar(copSprite, copLoc);

            // Initial Game State
            gameState = GameState.Menu;

            // Initial Speed
            speed = 1.0f;
            
            //dictionaries for the obstacle and levelTimers
            /*Dictionary<string, Queue<Obstacle>> levelQueues = new Dictionary<string, Queue<Obstacle>>();
            Dictionary<string, int> levelTimers = new Dictionary<string, int>(); */
            

            StreamReader streamReader = null;
            
            try
            {
                //if the file is read
                streamReader = new StreamReader("../../../LevelFile.txt");

                string line = "bananas";
                
                //reads until the file is null
                while (line != null)
                {
                    //All of this commented out stuff is in case we decide to change the level backgrounds again
                    
                    //reads the next line and sets it to the levelName
                    //string levelName = streamReader.ReadLine();

                    //if that line is null then breaks, basically checking the condition while looping
                    //line = levelName;
                    /*if (line == null)
                    {
                        break;
                    }*/

                    //creates a new obstacle queue for the level
                    //levelQueues[levelName] = new List<Obstacle>();
                    
                    //sets the level's time limit and places it in the dictionary
                    //int timer = int.Parse(streamReader.ReadLine());
                    //levelTimers[levelName] = timer;

                    //After the first two lines, keeps repeating until the line is ""


                    line = streamReader.ReadLine();

                    int i = 0;

                    while (line != null)
                    {
                        

                        if (line == "")
                        {
                            levelQueues.Add(new List<Obstacle>());
                            i++;
                        }
                        else
                        {
                        //Splits the line into an array of strings
                            string[] obstacleData = line.Split('|');

                        //creates an obstacleType variable from data from the array
                            ObstacleType obstacleType = (ObstacleType)int.Parse(obstacleData[0]);

                        //a key string for the obstacle texture dictionary
                            string obstacleKey = obstacleData[1];

                        //and an integer for the obstacle's timer
                            int obstacleTimer = int.Parse(obstacleData[2]);

                        //enqueue's the new object and calls the setUp method
                            levelQueues[i - 1].Add(new Obstacle(obstacleType, obsTextureDict[obstacleKey], (float)obstacleTimer, GraphicsDevice));
                            
                        }

                        line = streamReader.ReadLine();

                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            //closes the streamreader if it was open
            if (streamReader != null) { streamReader.Close(); }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            
            //Checking for button press
            foreach (Button b in buttons)
                {
                    b.Update(gameTime);
                }
            

            // TODO: Add your update logic here
            KeyboardState kbState = Keyboard.GetState();

            // FSM Implementation
            switch (gameState)
            {
                case GameState.Menu:
                    // Code in Menu

                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        Reset();
                        gameState = GameState.Game;
                    }     
                    if (SingleKeyPress(Keys.H, kbState))
                    {
                        gameState = GameState.HighScoreScreen;
                    }
                    break;

                //Case for the options menu inside menu
                case GameState.ControlsMenu:
                    if (SingleKeyPress(Keys.Escape, kbState))
                    {
                        gameState = GameState.Menu;
                    }
                    if (SingleKeyPress(Keys.H, kbState))
                    {
                        gameState = GameState.HighScoreScreen;
                    }
                    break;

                

                case GameState.Game:
                    // Code in Game

                    //checks to see if all the obstacles in the the current chunk have been completed
                    if (tempObstacle.CompleteList(levelQueues[currentObsList]))
                    {
                        //if they are, move on to the next list
                        currentObsList++;
                    }

                    //if the current obs list is greater than the number of obstacle lists
                    if (currentObsList >= levelQueues.Count)
                    {
                        //sets the current obs list back to zero and re randomizes the list.
                        currentObsList = 0;
                        RandomizeObstacles();

                        //resets the position of all obstacles so they're good to go
                        foreach (List<Obstacle> obsList in levelQueues)
                        {
                            foreach (Obstacle ob in obsList)
                            {
                                ob.Reset();
                            }
                        }
                    }
                    
                    //initializes the countdown on all the obstacles in the current list
                    foreach (Obstacle obs in levelQueues[currentObsList])
                    {
                        obs.InitializeCountdown();
                    }
                    



                    // Update Player
                    player.UpdateAnimation(gameTime);
                    player.Update(gameTime, speed);
                    UpdateSpeed();
                    player.UpdateDistance(gameTime, speed);
                    player.UpdateScore(gameTime, speed);

                    //If the cop is activated
                    if (cop.IsActive)
                    {
                        cop.Update(gameTime, player.PlayerDistance, speed);
                       
                    }

                    //if an obstacle is activated, updates it. Otherwise, decreases the send time
                    foreach (Obstacle obs in levelQueues[currentObsList])
                    {
                        if (obs.IsActive)
                        {
                            obs.Update(gameTime, speed);
                        }
                        else
                        {
                            obs.SendTime = obs.SendTime - 16.7f;
                        }
                    }
                    
                    
                    //pause the game
                    if (SingleKeyPress(Keys.Escape, kbState))
                    {
                        gameState = GameState.Pause;
                    }

                    //activate the cop
                    if (SingleKeyPress(Keys.T, kbState))
                    {
                        if (cop.IsActive)
                        {
                            cop.IsActive = false;
                        }
                        else
                        {
                            cop.Initialize(player.PlayerDistance);
                        }
                        
                    }

                    //throw an obstacle at the player
                    foreach (Obstacle obs in levelQueues[currentObsList])
                    {
                        if (obs.SendTime <= 0)
                        {
                            obs.Reset();
                            obs.Initialize();
                        }
                        
                    }

                    // Slow down player if they hit an obstacle
                    for (int i = 0; i < levelQueues[currentObsList].Count; i++)
                    {
                        if (levelQueues[currentObsList][i].IsActive && levelQueues[currentObsList][i].CheckCollision(player.PlayerLoc))
                        {
                            SlowDown(levelQueues[currentObsList][i]);
                            levelQueues[currentObsList][i].IsActive = false;
                        }
                    }
                    
                    // Temporary entry into Game Over
                    if (SingleKeyPress(Keys.R, kbState) || (cop.CheckCollision(player.PlayerDistance, player.PlayerLoc) && cop.IsActive))
                    {
                        if (player.PlayerScore > highScoreSystem.LowestScore())
                        {
                            gameState = GameState.SaveScore;
                        }
                        else
                        {
                            gameState = GameState.GameOver;
                        }

                    }

                    if (speed > 1.0)
                    {
                        //background update positions
                        backgroundScrollingPositions[0] -= (int)(1 + (speed * speed) / 4);
                        backgroundScrollingPositions[1] -= (int)(2 + (speed * speed) * 1.25f);
                        backgroundScrollingPositions[2] -= (int)(3 + (speed * speed) * 2.5f);
                        //ground update position
                        backgroundScrollingPositions[3] -= (int)(4 + (speed * speed) * 4);
                        //tunnel update position
                        backgroundScrollingPositions[4] -= (int)(4 + (speed * speed) * 4);
                    }
                    else
                    {
                        //background update positions
                        backgroundScrollingPositions[0] -= (int)(1 + (speed / 4));
                        backgroundScrollingPositions[1] -= (int)(2 + (speed * 1.25f));
                        backgroundScrollingPositions[2] -= (int)(3 + (speed * 2.5f));
                        //ground update position
                        backgroundScrollingPositions[3] -= (int)(4 + (speed * 4));
                        //tunnel update position
                        backgroundScrollingPositions[4] -= (int)(4 + (speed) * 4);
                    }

                    //reset tunnel position before time of day switch
                    if ((timeOfDayTimer > 80 && timeOfDayTimer < 90) || (timeOfDayTimer > 180 && timeOfDayTimer < 190) || (timeOfDayTimer > 280 && timeOfDayTimer < 290))
                    {
                        backgroundScrollingPositions[4] = 1600;
                    }

                    //update time of day
                    timeOfDayTimer += 0.025f; 
                    if (timeOfDayTimer>300)
                    {
                        timeOfDayTimer = 0;
                    }

                    

                    for (int i = 0; i < 4; i++)
                    {
                        //ground scrolling
                        if (i == 3)
                        {
                            if (backgroundScrollingPositions[i] < -128)
                            {
                                backgroundScrollingPositions[i] += 128;
                            }
                        }
                        //background scrolling
                        else
                        {
                            if (backgroundScrollingPositions[i] < -1536)
                            {
                                backgroundScrollingPositions[i] += 1536;

                            }
                        }
                    }
                    break;

                case GameState.Pause:
                    // Code in Pause

                    if (SingleKeyPress(Keys.Escape, kbState))
                    {
                        gameState = GameState.Game;
                    }

                    if (SingleKeyPress(Keys.Back, kbState))
                    {
                        gameState = GameState.Menu;
                    }
                    break;

                case GameState.GameOver:
                    // Code in Game Over

                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.HighScoreScreen:

                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.SaveScore:
                    /*
                    Keys[] keysPressed = kbState.GetPressedKeys();
                    
                    if (keysPressed.Length >0 )
                    {
                        Keys lastPressed = keysPressed[keysPressed.Length - 1];

                        if ((lastPressed >= Keys.A && lastPressed <= Keys.Z))
                        {
                            if (playername.Length > 0 && lastPressed == Keys.Back)
                            {
                                playername = playername.Substring(0, playername.Length - 1);
                            }
                            else if (playername.Length < 3 && !(lastPressed == Keys.Back))
                            {
                                playername += lastPressed.ToString();
                            }
                        }

                    }
                    */
                    //goes through a list of acceptaple keys
                    foreach (Keys key in letterKeys)
                    {
                        //and adds them to the player string if they were pressed once.
                        if (playerName.Length > 0 && SingleKeyPress(Keys.Back,kbState))
                        {
                            playerName = playerName.Substring(0, playerName.Length - 1);
                        }
                        //also allows for deletion of the playerName so the player can start again
                        if (playerName.Length < 3 && SingleKeyPress((Keys)key, kbState))
                        {
                            playerName += key.ToString();
                        }
                    }

                    //saves highscores, loads them again,
                    //and then goes to highscore once enter is pressed
                    if (SingleKeyPress(Keys.Enter, kbState))
                    {
                        HighScoreSystem.SaveHighScores(playerName,(int)player.PlayerScore);
                        HighScoreSystem.LoadHighScores();
                        gameState = GameState.HighScoreScreen;
                    }
                    break;
            }

            // Save current keyboard state to the previous keyboard state
            prevKBState = kbState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);

            _spriteBatch.Begin();

            // FSM Implementation
            switch (gameState)
            {
                case GameState.Menu:
                    //Buttons
                    //Main menu to game button
                    buttons.Add(new Button(_graphics.GraphicsDevice,
                        new Rectangle(667, 430, 208, 84),
                        font,
                        Color.Transparent));
                    buttons[0].LeftButtonClick += this.ChangeFromMenutoGame;
                    buttons[0].LeftButtonClick += this.Reset;

                    //Main menu to options
                    buttons.Add(new Button(_graphics.GraphicsDevice,
                            new Rectangle(635, 655, 270, 90),
                            font,
                            Color.Transparent));
                    buttons[1].LeftButtonClick += this.ChangeFromMenutoOptions;
                    buttons[1].LeftButtonClick += this.Reset;

                    //Draw background
                    for (int i = 0; i < 3; i++)
                    {
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i]);
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i] + 1536);
                    }

                    // Drawing menu and button
                    _spriteBatch.Draw(mainMenu, new Rectangle(1, 1, 1536, 864), Color.White);
                    
                    foreach (Button b in buttons)
                    {
                        b.Draw(_spriteBatch);
                    }                
                    break;

                case GameState.ControlsMenu:
                    //Draw background
                    for (int i = 0; i < 3; i++)
                    {
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i]);
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i] + 1536);
                    }
                    //Draw the menu 
                    _spriteBatch.Draw(controlsMenu, new Rectangle(1, 1, 1536, 864), Color.White);

                    _spriteBatch.DrawString(baseFont, "Esc to return to menu/pause", new Vector2(200, 800), Color.Maroon);
                    _spriteBatch.DrawString(baseFont, ("Press T to toggle Cop Car (Godmode)"), new Vector2(1000, 800), Color.Maroon);
                    _spriteBatch.DrawString(baseFont, "Press H to view high scores", new Vector2(600, 800), Color.Maroon);
                    break;

                case GameState.Game:
                    //Erases the button
                    buttons.Clear();

                    //draw background layers
                    for (int i = 0; i < 3; i++)
                    {
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i]);
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i] + 1536);
                    }

                    //draw ground layer
                    for (int i = 0; i<_graphics.PreferredBackBufferWidth/128 + 1; i++)
                    {
                        Draw128x128(_spriteBatch, 0, backgroundScrollingPositions[3]+i*128, _graphics.PreferredBackBufferHeight-128);
                    }

                    //draw tunnel
                    _spriteBatch.Draw(tunnelSprite, new Vector2(backgroundScrollingPositions[4],0), Color.White);

                    //draw score UI
                    if (speed>0)
                    {
                        DrawGUI(_spriteBatch, 0);
                    }
                    if (speed > 1)
                    {
                        DrawGUI(_spriteBatch, 1);
                    }
                    if (speed > 1.3)
                    {
                        DrawGUI(_spriteBatch, 2);
                    }
                    if (speed > 1.6)
                    {
                        DrawGUI(_spriteBatch, 3);
                    }
                    if (speed > 1.9)
                    {
                        DrawGUI(_spriteBatch, 4);
                    }





                    player.Draw(_spriteBatch, speed);
                    //_spriteBatch.DrawString(baseFont, string.Format("Speed: {0:F2}" ,speed), new Vector2(100, 100), Color.Maroon);
                    //_spriteBatch.DrawString(baseFont, "Distance: " + player.PlayerDistance, new Vector2(100, 200), Color.Black);
                    _spriteBatch.DrawString(scoreFont,$"{(int)player.PlayerScore}", new Vector2(600, 796), Color.Red);
                    //_spriteBatch.DrawString(baseFont, "Cop Distance: " + cop.CopDistance, new Vector2(100, 300), Color.Black);
                    //_spriteBatch.DrawString(baseFont, 
                      //  "Left Arrow to Go Left, Right Arrow to go Right, Space Bar to Jump, T to disable the Cop Car (aka godmode)", 
                       // new Vector2(300, 100), Color.Maroon);
                  //  _spriteBatch.DrawString(baseFont,
                     //   "R to Instantly Game Over, Esc to Pause, O to Options",
                     //   new Vector2(300, 120), Color.Maroon);

                    //Draws cop if they are active
                    if (cop.IsActive)
                    {
                        cop.Draw(_spriteBatch);
                    }

                    //draws all active obstacles
                    foreach (Obstacle obs in levelQueues[currentObsList])
                    {
                        if (obs.IsActive)
                        {
                            obs.Draw(_spriteBatch);
                        }
                    }
                    

                    break;

                case GameState.Pause:
                    //Drawing pause menu
                    //Main menu to pause                    
                    //Draw the pause menu sprite
                    //Draw background
                    for (int i = 0; i < 3; i++)
                    {
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i]);
                        DrawBackground(_spriteBatch, i, backgroundScrollingPositions[i] + 1536);
                    }
                    _spriteBatch.Draw(pauseMenu, new Rectangle(1, 1, 1536, 864), Color.White);
                    break;

                case GameState.GameOver:
                    //Drawing game over
                    _spriteBatch.Draw(gameOver, new Rectangle(1, 1, 1536, 864), Color.White);
                    _spriteBatch.DrawString(baseFont, string.Format("Score: {0:F0}", player.PlayerScore), new Vector2(700, 700), Color.Maroon);
                    _spriteBatch.DrawString(baseFont, "PRESS ENTER TO RETURN TO MENU", new Vector2(700, 600), Color.Maroon);
                    break;
                case GameState.HighScoreScreen:
                    _spriteBatch.DrawString(scoreFont, string.Format("HIGHSCORES"), new Vector2(600, 70), Color.Maroon);
                    _spriteBatch.DrawString(scoreFont, "PRESS ENTER TO RETURN TO MENU", new Vector2(340, 780), Color.Maroon);
                    Vector2 drawLocation = new Vector2(1536, 864);
                    HighScoreSystem.DrawHighScores(_spriteBatch, scoreFont);

                    break;
                case GameState.SaveScore:
                    _spriteBatch.Draw(gameOver, new Rectangle(1, 1, 1536, 864), Color.White);
                    _spriteBatch.DrawString(scoreFont, string.Format("SCORE: {0:F0}", player.PlayerScore), new Vector2(700, 600), Color.Maroon);
                    _spriteBatch.DrawString(scoreFont, "ENTER 3 LETTERS AND PRESS ENTER:", new Vector2(300, 660), Color.Maroon);
                    _spriteBatch.DrawString(scoreFont, string.Format("{0}",playerName.ToUpper()), new Vector2(1300, 660), Color.Maroon);
                    break;
            }

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
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

        /// <summary>
        /// A method which returns the speed at which the game should be moving
        /// </summary>
        /// <returns> Speed </returns>
        public float UpdateSpeed()
        {
            // Create a temporary copy of speed
            float pastSpeed = speed;

            // Set new speed
            speed = (float)player.PlayerLoc.X / (GraphicsDevice.Viewport.Width / 2 - 128);

            // Factor in delays
            for (int d = 0; d < delays.Count; d++)
            {
                // Reduce speed by the delay
                speed -= delays[d][0];

                // Update the remaining time left for the delay
                delays[d][1]--;

                // Remove delay if it has worn out
                if (delays[d][1] <= 0) 
                {
                    delays.Remove(delays[d]);

                }
            }

            // Gameplay quality speed checks
            // Set a minimum speed
            if (speed < 0.3) speed = 0.3f;

            // Cap speed change per frame
            if (pastSpeed + 0.5f < speed)
            {
                speed = pastSpeed + 0.5f;
            }
            else if (pastSpeed - 0.5f > speed)
            {
                speed = pastSpeed - 0.5f;
            }

            return speed;
        }

        /// <summary>
        /// Creates a new delay amount and duration based on an obstacle's weight
        /// Adds it to the delays list
        /// </summary>
        /// <param name="obs"> obstacle that collided with the player </param>
        private void SlowDown(Obstacle obs)
        {
            delays.Add(new float[2] { 3 * obs.Weight, obs.Weight * 320 });  // Will be chaged back to 2x
        } 



        public void ChangeFromMenutoGame()
        {
            gameState = GameState.Game;
        }

        public void ChangeFromMenutoOptions()
        {
            gameState = GameState.ControlsMenu;
        }

        /// <summary>
        /// method to reset the game when returning to menu
        /// </summary>
        public void Reset() 
        {
            player.PlayerReset(playerLoc);
            cop.CopReset(copLoc);
            cop.Initialize(player.PlayerDistance);
            cop.IsActive = true;
            RandomizeObstacles();
            player.PlayerScore = 0;
            foreach (List<Obstacle> obstacleList in levelQueues)
            {
                foreach (Obstacle obs in obstacleList)
                {
                    obs.Reset();
                    obs.IsActive = false;
                }
            }    
            
        }

        public void DrawBackground(SpriteBatch spriteBatch, int frame, float x)
        {
            
            Texture2D currentBackground = backgroundSpriteSheet;

            if (timeOfDayTimer>100)
            {
                currentBackground = backgroundSpriteSheetEvening;
            }
            if (timeOfDayTimer>200)
            {
                currentBackground = backgroundSpriteSheetNight;
            }
            spriteBatch.Draw(
                currentBackground,                            // - The texture to draw
                new Vector2(x,0), // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame*1536,                                  //   - This rectangle specifies
                    0,                   //	   where "inside" the texture
                    1536,                     //     to get pixels (We don't want to
                    864),                   //     draw the whole thing)
                Color.White,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                0,                             // - Can be used to flip the image
                0);
        }
        public void Draw128x128(SpriteBatch spriteBatch, int frame, float x, float y)
        {
            int SourceRectangleY = 0;
            if (frame>7 && SourceRectangleY==0)
            {
                SourceRectangleY = 128;
                frame -=8;
            }
            spriteBatch.Draw(
                assetSheet128x128,                            // - The texture to draw
                new Vector2(x, y), // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * 128,                                  //   - This rectangle specifies
                    SourceRectangleY,                   //	   where "inside" the texture
                    128,                     //     to get pixels (We don't want to
                    128),                   //     draw the whole thing)
                Color.White,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                0,                             // - Can be used to flip the image
                0);
        }

        public void DrawGUI(SpriteBatch spriteBatch, int frame)
        {
            spriteBatch.Draw(
                gameUI,                            // - The texture to draw
                new Vector2(_graphics.PreferredBackBufferWidth/2-328, _graphics.PreferredBackBufferHeight-96), // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * 656,                                  //   - This rectangle specifies
                    0,                   //	   where "inside" the texture
                    656,                     //     to get pixels (We don't want to
                    96),                   //     draw the whole thing)
                Color.White,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                0,                             // - Can be used to flip the image
                0);
        }

        /// <summary>
        /// A method which randomizes the order of the obstacle chunks using the Fisher-Yates shuffle
        /// </summary>
        public void RandomizeObstacles()
        {
            int n = levelQueues.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n - 1);
                List<Obstacle> value = levelQueues[k];
                levelQueues[k] = levelQueues[n];
                levelQueues[n] = value;
            }
        }

        
    }
}