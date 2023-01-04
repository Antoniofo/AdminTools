using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Life;
using Life.Network;
using UnityEngine;

namespace AdminTools
{
    public class AdminToolsPlugin : Plugin
    {
        public AdminToolsPlugin(IGameAPI api) : base(api)
        {
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();


            SChatCommand pos = new SChatCommand("/pos", "get pos", "/pos", async (player, args) =>
            {
                if (player.IsAdmin)
                {
                    await player.Save();
                    player.SendText($"X:{player.character.LastPosX} Y:{player.character.LastPosY} Z:{player.character.LastPosZ}");
                    Debug.Log($"X:{player.character.LastPosX} Y:{player.character.LastPosY} Z:{player.character.LastPosZ}");
                }
                else
                {
                    player.SendText($"Your not an admin");
                }
                

            });

            SChatCommand bcr = new SChatCommand("/bcr","remove and regive", "/bcr", (player, args) =>
            {
                if (player.IsAdmin)
                {
                    player.character.HasBCR = !player.character.HasBCR;
                    player.SendText($"Votre BCR: {player.character.HasBCR}");                    
                }
                else
                {
                    player.SendText($"Your not an admin");
                }


            });

            SChatCommand tp = new SChatCommand("/tp", "teleport to coordinate", "/tp x y z", (player, args) =>
            {
                if (player.IsAdmin)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    try
                    {
                        x = float.Parse(args[0]);
                        y = float.Parse(args[1]);
                        z = float.Parse(args[2]);
                    }
                    catch {
                        player.SendText("Error parsing ");
                        return;
                    }

                    player.setup.TargetSetPosition(new Vector3(x,y,z));
                    player.SendText($"X:{x} Y:{y} Z:{z}");
                }
                else
                {
                    player.SendText($"Your not an admin");
                }


            });
            bcr.Register();
            tp.Register();
            pos.Register();
        }

        

    }
}
