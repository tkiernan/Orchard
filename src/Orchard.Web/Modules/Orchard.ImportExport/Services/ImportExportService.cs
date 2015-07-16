﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Recipes.Services;
using Orchard.Services;

namespace Orchard.ImportExport.Services {
    public class ImportExportService : IImportExportService {
        private readonly IOrchardServices _orchardServices;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IRecipeBuilder _recipeBuilder;
        private readonly IRecipeExecutor _recipeExecutor;
        private readonly IClock _clock;
        private const string ExportsDirectory = "Exports";

        public ImportExportService(
            IOrchardServices orchardServices,
            IAppDataFolder appDataFolder,
            IRecipeBuilder recipeBuilder, 
            IRecipeExecutor recipeExecutor, 
            IClock clock) {

            _orchardServices = orchardServices;
            _appDataFolder = appDataFolder;
            _recipeBuilder = recipeBuilder;
            _recipeExecutor = recipeExecutor;
            _clock = clock;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public string Import(XDocument recipeDocument) {
            return _recipeExecutor.Execute(recipeDocument);
        }

        public string Import(string recipeText) {
            return _recipeExecutor.Execute(recipeText);
        }

        public XDocument ExportXml(IEnumerable<IRecipeBuilderStep> steps) {
            var recipe = _recipeBuilder.Build(steps);
            return recipe;
        }

        public string Export(IEnumerable<IRecipeBuilderStep> steps) {
            var recipe = _recipeBuilder.Build(steps);
            return recipe.ToString();
        }

        public string WriteExportFile(XDocument recipeDocument) {
            return WriteExportFile(recipeDocument.ToString());
        }

        public string WriteExportFile(string recipeText) {
            var exportFile = String.Format("Export-{0}-{1}.xml", _orchardServices.WorkContext.CurrentUser.UserName, _clock.UtcNow.Ticks);
            if (!_appDataFolder.DirectoryExists(ExportsDirectory)) {
                _appDataFolder.CreateDirectory(ExportsDirectory);
            }

            var path = _appDataFolder.Combine(ExportsDirectory, exportFile);
            _appDataFolder.CreateFile(path, recipeText);

            return _appDataFolder.MapPath(path);
        }
    }
}