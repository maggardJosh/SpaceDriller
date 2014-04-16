using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	public class Player : FSprite
	{
        float speed = 200.0f;
        public Player() : base("character")
        {
            
        }

        public override void HandleAddedToStage()
        {
            Futile.instance.SignalUpdate += Update;
            base.HandleAddedToStage();
        }

        public override void HandleRemovedFromStage()
        {
            Futile.instance.SignalUpdate -= Update;
            base.HandleRemovedFromStage();
        }

        private void Update()
        {
            if (Input.GetKey(UnityEngine.KeyCode.W))
                this.y += Time.deltaTime * speed;
            else if (Input.GetKey(UnityEngine.KeyCode.S))
                this.y -= Time.deltaTime * speed;

            if (Input.GetKey(KeyCode.A))
                this.x -= Time.deltaTime * speed;
            else
                if (Input.GetKey(KeyCode.D))
                    this.x += Time.deltaTime * speed;
        }

	}
