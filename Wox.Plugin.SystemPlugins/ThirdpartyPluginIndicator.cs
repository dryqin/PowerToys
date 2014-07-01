﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Infrastructure;
using Wox.Infrastructure.Storage;
using Wox.Infrastructure.Storage.UserSettings;

namespace Wox.Plugin.SystemPlugins
{
    public class ThirdpartyPluginIndicator : BaseSystemPlugin
    {
        private List<PluginPair> allPlugins = new List<PluginPair>();
        private Action<string> changeQuery;

        protected override List<Result> QueryInternal(Query query)
        {
            List<Result> results = new List<Result>();

            foreach (PluginMetadata metadata in allPlugins.Select(o => o.Metadata))
            {
                if (metadata.ActionKeyword.StartsWith(query.RawQuery))
                {
                    PluginMetadata metadataCopy = metadata;
                    var customizedPluginConfig = UserSettingStorage.Instance.CustomizedPluginConfigs.FirstOrDefault(o => o.ID == metadataCopy.ID);
                    if (customizedPluginConfig != null && customizedPluginConfig.Disabled)
                    {
                        continue;
                    }

                    Result result = new Result
                    {
                        Title = metadata.ActionKeyword,
                        SubTitle = string.Format("Activate {0} plugin", metadata.Name),
                        Score = 50,
                        IcoPath = "Images/work.png",
                        Action = (c) =>
                        {
                            changeQuery(metadataCopy.ActionKeyword + " ");
                            return false;
                        },
                    };
                    results.Add(result);
                }
            }

            results.AddRange(UserSettingStorage.Instance.WebSearches.Where(o => o.ActionWord.StartsWith(query.RawQuery) && o.Enabled).Select(n => new Result()
            {
                Title = n.ActionWord,
                SubTitle = string.Format("Activate {0} web search", n.ActionWord),
                Score = 50,
                IcoPath = "Images/work.png",
                Action = (c) =>
                {
                    changeQuery(n.ActionWord + " ");
                    return false;
                }
            }));

            return results;
        }

        protected override void InitInternal(PluginInitContext context)
        {
            allPlugins = context.Plugins;
            changeQuery = context.ChangeQuery;
        }


        public override string ID
        {
            get { return "6A122269676E40EB86EB543B945932B9"; }
        }

        public override string Name
        {
            get { return "Third-party Plugin Indicator"; }
        }

        public override string IcoPath
        {
            get { return @"Images\list.png"; }
        }

        public override string Description
        {
            get { return "Provide Third-party plugin actionword suggeestion."; }
        }
    }
}
