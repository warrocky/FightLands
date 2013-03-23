using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class FightManager : GameObject
    {
        public GameManager gameManager;
        FightWorld fightWorld;

        ActiveBox activeBox;

        public enum FightState { idle, starting, fighting, ending };
        public FightState fightState;

        public float fadeTime;

        public float fightTime;

        public delegate void FightOverHandler();
        public event FightOverHandler fightOver;

        Creature playerCreature;
        Creature enemyCreature;


        public FightManager(GameManager gameManager)
            :base(gameManager.world)
        {
            fightState = FightState.idle;
            fightOver += EndFight;
            this.gameManager = gameManager;
        }
        public void StartFight(Creature playerCreature, Creature otherCreature, Land land, Vector2 position)
        {
            fightWorld = new FightWorld(land, position, playerCreature, otherCreature);

            this.playerCreature = playerCreature;
            this.enemyCreature = otherCreature;

            Rectangle rect = gameManager.getActiveBoxArea();
            activeBox = new ActiveBox(gameManager.world, new Camera(rect.Width, rect.Height, fightWorld));
            activeBox.position = UserInterfaceManager.getUserInterfaceArea("activebox").getCenter();
            activeBox.texture.filter = Color.Transparent;

            fadeTime = 1f;
            fightState = FightState.starting;
        }
        private void EndFight()
        {
            fightState = FightState.idle;
            enemyCreature.destroy();
            fightWorld = null;
            
            activeBox.destroy();
            gameManager.FightEnded();
        }
        public override void Update(UpdateState state)
        {
            switch (fightState)
            {
                case FightState.starting:
                    if(fadeTime == 1f)
                        fightWorld.Update(state);

                    fadeTime -= state.elapsedTime;
                    activeBox.texture.filter = Color.Lerp(Color.White, Color.Transparent, fadeTime);
                    if (fadeTime <= 0f)
                        fightState = FightState.fighting;

                    break;
                case FightState.fighting:
                    fightWorld.Update(state);
                    fightTime += state.elapsedTime;

                    if (fightTime >= 30f)
                    {
                        fightTime = 0f;
                        EndFight();
                    }
                    break;
            }
        }
    }
}
