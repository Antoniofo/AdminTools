using System.Collections.Generic;
using System.IO;
using Crosstales.Common.Util;
using Life;
using Life.Network;
using Life.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace AdminTools
{
    public class AdminToolsPlugin : Plugin
    {
        private static string _dirPath;
        public static string ConfPath;
        private AdminToolsConfig _config;

        public AdminToolsPlugin(IGameAPI api) : base(api)
        {
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            InitDirectory();

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
                    }
                    else
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
                (player, strings) =>
                {
                    if (player.IsAdmin)
                    {
                        UIPanel tpui = new UIPanel("Teleport Panel", UIPanel.PanelType.Tab)
                            .AddButton("Fermer", player.ClosePanel)
                            .AddButton("Teleport", (ui) => ui.SelectTab());

                        try
                        {
                            foreach (var area in Nova.server.areas.areas)
                            {
                                if (_config.Areas[area.id])
                                {
                                    tpui.AddTabLine($"({area.id}) {area.name}",
                                        (ui) => { player.setup.TargetSetPosition(area.spawn); });
                                }
                            }
                        }
                        catch
                        {
                            foreach (var area in Nova.server.areas.areas)
                            {
                                if (!_config.Areas.ContainsKey(area.id))
                                    _config.Areas.Add(area.id, true);
                            }

                            File.WriteAllText(ConfPath, JsonConvert.SerializeObject(_config));

                            foreach (var area in Nova.server.areas.areas)
                            {
                                if (_config.Areas[area.id])
                                {
                                    tpui.AddTabLine($"({area.id}) {area.name}",
                                        (ui) => { player.setup.TargetSetPosition(area.spawn); });
                                }
                            }
                        }

                        player.ShowPanelUI(tpui);
                    }
                    else
                    {
                        player.SendText("Your not an admin");
                    }
                });
            SChatCommand reload = new SChatCommand("/admintoolsreload", "reload config from AdminTools",
                "/admintoolsreload",
                (player, arg) =>
                {
                    _config = JsonConvert.DeserializeObject<AdminToolsConfig>(File.ReadAllText(ConfPath));
                    tpmenu.action = (player1, strings) =>
                    {
                        if (player.IsAdmin)
                        {
                            UIPanel tpui = new UIPanel("Teleport Panel", UIPanel.PanelType.Tab)
                                .AddButton("Fermer", player.ClosePanel)
                                .AddButton("Teleport", (ui) => ui.SelectTab());

                            try
                            {
                                foreach (var area in Nova.server.areas.areas)
                                {
                                    if (_config.Areas[area.id])
                                    {
                                        tpui.AddTabLine($"({area.id}) {area.name}",
                                            (ui) => { player.setup.TargetSetPosition(area.spawn); });
                                    }
                                }
                            }
                            catch
                            {
                                foreach (var area in Nova.server.areas.areas)
                                {
                                    if (!_config.Areas.ContainsKey(area.id))
                                        _config.Areas.Add(area.id, true);
                                }

                                File.WriteAllText(ConfPath, JsonConvert.SerializeObject(_config));

                                foreach (var area in Nova.server.areas.areas)
                                {
                                    if (_config.Areas[area.id])
                                    {
                                        tpui.AddTabLine($"({area.id}) {area.name}",
                                            (ui) => { player.setup.TargetSetPosition(area.spawn); });
                                    }
                                }
                            }

                            player.ShowPanelUI(tpui);
                        }
                        else
                        {
                            player.SendText("Your not an admin");
                        }
                    };
                    player.SendText("Reload successful");
                });
            reload.Register();
            tpmenu.Register();
            bcr.Register();
            tp.Register();
            pos.Register();
        }

        private void InitDirectory()
        {
            _dirPath = $"{pluginsPath}/AdminTools";
            ConfPath = _dirPath + "/config.json";

            if (!Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);
            if (!File.Exists(ConfPath))
            {
                _config = new AdminToolsConfig() { Areas = new SerializableDictionary<uint, bool>() };
                foreach (var area in Nova.server.areas.areas)
                {
                    _config.Areas.Add(area.id, true);
                }

                File.WriteAllText(ConfPath, JsonConvert.SerializeObject(_config));
            }
            else
            {
                _config = JsonConvert.DeserializeObject<AdminToolsConfig>(File.ReadAllText(ConfPath));
            }
        }
    }

    [System.Serializable]
    public class AdminToolsConfig
    {
        public SerializableDictionary<uint, bool> Areas;

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(AdminToolsPlugin.ConfPath, json);
        }
    }
}