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


            SChatCommand pos = new SChatCommand("/pos", "Show your position in the chat and the console", "/pos", (player, args) =>
            {
                if (player.IsAdmin)
                {                    
                    player.SendText($"X:{player.setup.transform.position.x} Y:{player.setup.transform.position.y} Z:{player.setup.transform.position.z}");
                    player.SendText($"X:{player.setup.transform.rotation.x} Y:{player.setup.transform.rotation.y} Z:{player.setup.transform.rotation.z} W:{player.setup.transform.rotation.w}");
                    Debug.Log($"X:{player.setup.transform.position.x} Y:{player.setup.transform.position.y} Z:{player.setup.transform.position.z}");
                    Debug.Log($"X:{player.setup.transform.rotation.x} Y:{player.setup.transform.rotation.y} Z:{player.setup.transform.rotation.z} W:{player.setup.transform.rotation.w}");                                        
                }
                
                else
                {
                    player.SendText($"Your not an admin");
                }
                

            });

            SChatCommand bcr = new SChatCommand("/bcr", "Give you the BCR or remove if you have it", "/bcr", (player, args) =>
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

            SChatCommand tp = new SChatCommand("/tp", "Telepot you to the coordinate you've entered", "/tp <x> <y> <z>", (player, args) =>
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
