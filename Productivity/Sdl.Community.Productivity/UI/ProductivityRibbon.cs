﻿using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Productivity.UI
{
    [RibbonGroup("Sdl.Community.Productivity", Name = "Community Productivity")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class ProductivityRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.Productivity", Icon = "icon",Name = "Productivity score", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    class ProductivityViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            var twitterPersistenceService = new TwitterPersistenceService(logger);

            if (!ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService, logger))
            {
                MessageBox.Show(PluginResources.ProductivityViewPartAction_Execute_You_need_to_configure_the_twitter_account_to_see_your_score);
                return;
            }
            using (var pForm = new ProductivityForm())
            {
                pForm.ShowDialog();
            }
        }
    }

    [Action("Sdl.Community.ProductivityShare",  Icon = "twitter", Name = "Share", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    class ProductivityShareViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            var twitterPersistenceService = new TwitterPersistenceService(logger);
            if (!ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService,logger))
            {
                MessageBox.Show(
                     PluginResources
                         .ProductivityShareViewPartAction_Execute_In_order_to_share_the_result_you_need_to_configure_your_twitter_account);
                return;
            }
            var productivityService = new ProductivityService(logger);

            if (productivityService.TotalNumberOfCharacters < Constants.MinimumNumberOfCharacters)
            {
                MessageBox.Show(
                    string.Format(
                        PluginResources
                            .ProductivityShareViewPartAction_Execute_In_order_to_share_your_score_you_need_to_translate_at_least__0__characters,
                        Constants.MinimumNumberOfCharacters.ToString("N0")));
                return;
                
            }

            var shareService = new ShareService(productivityService, twitterPersistenceService, logger);
            shareService.Share();

        }

       
    }
}
