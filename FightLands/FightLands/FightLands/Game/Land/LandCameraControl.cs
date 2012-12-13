using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandCameraControl : GameObject
    {
        Camera camera;
        GameObject anchor;

        public LandCameraControl(Land land, Camera camera)
            : base(land)
        {
            this.camera = camera;
        }
        public override void Update(UpdateState state)
        {
            if (anchor != null)
            {
                camera.position += (anchor.position - camera.position) * 3f * state.elapsedTime;
            }
        }
        public void setAnchor(GameObject gameObject)
        {
            this.anchor = gameObject;
        }
    }
}
