using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace TeamSk85GameProject
{
    //delegate for when the button is pressed
    public delegate void OnButtonClickDelegate();
    internal class Button
    {
        //fields
        protected SpriteFont font;
        protected MouseState prevMState;
        protected bool enabled = true;
        protected Rectangle position;       
        Texture2D buttonImg;

        //event for the left button click
        public event OnButtonClickDelegate LeftButtonClick;

        /// <summary>
        /// Constructor for a button object
        /// </summary>
        /// <param name="device">The graphics device used for making the custon 2D Texture</param>
        /// <param name="position">The position of the button</param>
        /// <param name="font">the font that the button's text uses</param>
        /// <param name="color">The color of the button</param>
        public Button(GraphicsDevice device, Rectangle position, SpriteFont font, Color color)
        {
            // Save copies/references to the info we'll need later
            this.font = font;
            this.position = position;

            // Code for position of text inside the button
            //Vector2 textSize = font.MeasureString(text);
            //textLoc = new Vector2(
             //   (position.X + position.Width / 2) - textSize.X / 2,
              //  (position.Y + position.Height / 2) - textSize.Y / 2
            //);

            // Make a custom 2d texture for the button itself
            buttonImg = new Texture2D(device, position.Width, position.Height, false, SurfaceFormat.Color);
            int[] colorData = new int[buttonImg.Width * buttonImg.Height];
            Array.Fill<int>(colorData, (int)color.PackedValue);
            buttonImg.SetData<Int32>(colorData, 0, colorData.Length);
        }
        /// <summary>
        /// Update method for updating the button
        /// </summary>
        /// <param name="gameTime">The gameTime</param>
        public void Update(GameTime gameTime)
        {
            // Check/capture the mouse state regardless of whether this button
            // if active so that it's up to date next time!
            MouseState mState = Mouse.GetState();
            if (mState.LeftButton == ButtonState.Released &&
                prevMState.LeftButton == ButtonState.Pressed &&
                this.position.Contains(mState.Position))
            {
                if (LeftButtonClick != null)
                {
                    LeftButtonClick();
                }

            }
            prevMState = mState;
        }

        /// <summary>
        /// Draw just the button
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonImg, position, Color.White);
        }
    }
}
