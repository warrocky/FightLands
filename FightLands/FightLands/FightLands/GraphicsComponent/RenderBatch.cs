using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands.GraphicsComponent
{
    class RenderBatch
    {
        protected List<DrawCall> drawCalls;
        protected List<RenderBatch> subBatchs;
        public readonly RenderTarget2D backbuffer;

        public RenderBatch(int width, int height)
        {
            backbuffer = new RenderTarget2D(Graphics.device, width, height);
            subBatchs = new List<RenderBatch>();
            drawCalls = new List<DrawCall>();
        }
        public void addSubBatch(RenderBatch subBatch)
        {
            subBatchs.Add(subBatch);
        }
        public void addDrawCall(DrawCall call)
        {
            drawCalls.Add(call);
        }
        protected internal void Render(RenderState state)
        {
            for(int i=0;i<subBatchs.Count;i++)
                subBatchs[i].Render(state);


            Graphics.device.SetRenderTarget(backbuffer);
            state.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < drawCalls.Count; i++)
                drawCalls[i].Render(state);
            state.spriteBatch.End();
            drawCalls.Clear();
        }
    }
}
