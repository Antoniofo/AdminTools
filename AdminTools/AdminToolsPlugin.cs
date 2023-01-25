using Life;
using Life.Network;
using Life.UI;
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

            foreach (var area in Nova.server.areas.areas)
            {
                Debug.Log($"({area.id}) {area.name}");
            }

            SChatCommand pos = new SChatCommand("/pos", "Show your position in the chat and the console", "/pos",
                (player, args) =>
                {
                    if (player.IsAdmin)
                    {
                        var transform = player.setup.transform;
                        var position = transform.position;
                        var rotation = transform.rotation;
                        player.SendText($"X:{position.x} Y:{position.y} Z:{position.z}");
                        player.SendText($"X:{rotation.x} Y:{rotation.y} Z:{rotation.z} W:{rotation.w}");
                        Debug.Log($"X:{position.x} Y:{position.y} Z:{position.z}");
                        Debug.Log($"X:{rotation.x} Y:{rotation.y} Z:{rotation.z} W:{rotation.w}");
                    }else
                    {
                        player.SendText($"Your not an admin");
                    }
                });

            SChatCommand bcr = new SChatCommand("/bcr", "Give you the BCR or remove if you have it", "/bcr",
                (player, args) =>
                {
                    if (player.IsAdmin)
                    {
                        player.character.HasBCR = !player.character.HasBCR;
                        player.SendText($"Your BCR: {player.character.HasBCR}");
                    }
                    else
                    {
                        player.SendText($"Your not an admin");
                    }
                });

            SChatCommand tp = new SChatCommand("/tp", "Telepot you to the coordinate you've entered", "/tp <x> <y> <z>",
                (player, args) =>
                {
                    if (player.IsAdmin)
                    {
                        float x;
                        float y;
                        float z;
                        try
                        {
                            x = float.Parse(args[0]);
                            y = float.Parse(args[1]);
                            z = float.Parse(args[2]);
                        }
                        catch
                        {
                            player.SendText("Error parsing");
                            return;
                        }

                        player.setup.TargetSetPosition(new Vector3(x, y, z));
                        player.SendText($"X:{x} Y:{y} Z:{z}");
                    }
                    else
                    {
                        player.SendText($"Your not an admin");
                    }
                });

            SChatCommand tpmenu = new SChatCommand("/tpmenu", "Opens a menu to teleport you to areas", "/tpmenu",
                (player, arg) =>
                {
                    if (player.IsAdmin)
                    {
                        UIPanel tpui = new UIPanel("Teleport Panel", UIPanel.PanelType.Tab)
                            .AddButton("Fermer", player.ClosePanel)
                            .AddButton("Teleport", (ui) => ui.SelectTab());
                        foreach (var area in Nova.server.areas.areas)
                        {
                            tpui.AddTabLine($"({area.id}) {area.name}",
                                (ui) => { player.setup.TargetSetPosition(area.spawn); });
                        }

                        player.ShowPanelUI(tpui);
                    }
                    else
                    {
                        player.SendText("Your not an admin");
                    }
                });
            tpmenu.Register();
            bcr.Register();
            tp.Register();
            pos.Register();
        }
    }
}