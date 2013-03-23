using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class FightWorld : World
    {
        Land land;
        Vector2 landPosition;

        FightCreature humanFighter;
        public FightCreature AIFighter;

        public float floorLevel;

        public FightWorld(Land land, Vector2 landPosition, Creature humanFightCreature, Creature AIFightCreature)
        {
            this.land = land;
            this.landPosition = landPosition;

            humanFighter = humanFightCreature.createFightCreature(this);
            humanFighter.position.X = -300f;
            PlayerManager.getPlayer("player1").addControlable(humanFighter);
            AIFighter = AIFightCreature.createFightCreature(this);
            AIFighter.position.X = 300f;

            floorLevel = 0f;

            //ground
            Dummy dum = new Dummy(this, "whiteSquare");
            dum.texture.filter = Color.Brown;
            dum.texture.size = new Vector2(1000f, 500f);
            dum.position.Y = 300f;

            //sky
            dum = new Dummy(this, "whiteSquare");
            dum.texture.filter = Color.SkyBlue;
            dum.texture.size = new Vector2(1000f, 600f);
            dum.position.Y = -250f;
        }
        public override void Draw(DrawState state)
        {
            base.Draw(state);
        }
    }
}
