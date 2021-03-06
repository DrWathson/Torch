﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamSDK;
using Sandbox;

namespace Torch
{
    /// <summary>
    /// SNAGGED FROM PHOENIX84'S SE WORKSHOP TOOL
    /// Keen's steam service calls RestartIfNecessary, which triggers steam to think the game was launched
    /// outside of Steam, which causes this process to exit, and the game to launch instead with an arguments warning.
    /// We have to override the default behavior, then forcibly set the correct options.
    /// </summary>
    public class SteamService : MySteamService
    {
        public SteamService(bool isDedicated, uint appId) : base(true, appId)
        {
            // TODO: Add protection for this mess... somewhere
            SteamServerAPI.Instance.Dispose();
            var steam = typeof(MySteamService);
            steam.GetField("SteamServerAPI").SetValue(this, null);

            steam.GetProperty("AppId").GetSetMethod(true).Invoke(this, new object[] { appId });
            if (isDedicated)
            {
                steam.GetField("SteamServerAPI").SetValue(this, SteamServerAPI.Instance);
            }
            else
            {
                var steamApi = SteamAPI.Instance;
                steam.GetField("SteamAPI").SetValue(this, SteamAPI.Instance);
                steam.GetProperty("IsActive").GetSetMethod(true).Invoke(this, new object[] { SteamAPI.Instance != null });

                if (steamApi != null)
                {
                    steam.GetProperty("UserId").GetSetMethod(true).Invoke(this, new object[] { steamApi.GetSteamUserId() });
                    steam.GetProperty("UserName").GetSetMethod(true).Invoke(this, new object[] { steamApi.GetSteamName() });
                    steam.GetProperty("OwnsGame").GetSetMethod(true).Invoke(this, new object[] { steamApi.HasGame() });
                    steam.GetProperty("UserUniverse").GetSetMethod(true).Invoke(this, new object[] { steamApi.GetSteamUserUniverse() });
                    steam.GetProperty("BranchName").GetSetMethod(true).Invoke(this, new object[] { steamApi.GetBranchName() });
                    steamApi.LoadStats();
                }
            }
        }
    }
}
