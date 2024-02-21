using Localization.Extensions;
using Localization.ExceptionResult;
using Localization.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Localization
{
    /// <summary>
    /// Object to generate .incaloc files.
    /// </summary>
    public class IncaLocCsvGenerator : IncaLocBase, IIncaLocGenerator
    {
        /// <summary>
        /// All the <see cref="CultureInfo"/> that will be added to the .incaloc.csv files generated.
        /// </summary>
        public IEnumerable<CultureInfo> Cultures { get; }

        /// <summary>
        /// The path of the folder in which the .incaloc.csv files will be saved. 
        /// </summary>
        public string StoreLocation { get; }

        /// <summary>
        /// The path of the .csproj file in which the .incaloc files will be added as embedded resources.
        /// </summary>
        public string ProjectFile { get; }

        /// <summary>
        /// Create an instance of <see cref="IncaLocGenerator"/> and initialize the properties.
        /// </summary>
        /// <param name="storeLocation">The path of the folder in which the .incaloc.csv files will be saved. </param>
        /// <param name="projectFile">The path of the .csproj file in which the .incaloc files will be added as embedded resources.</param>
        /// <param name="cultureInfos">All the <see cref="CultureInfo"/> that will be added to the .incaloc.csv files generated.</param>
        public IncaLocCsvGenerator(Uri storeLocation, Uri projectFile, IEnumerable<CultureInfo>? cultureInfos = null)
        {
            StoreLocation = storeLocation.LocalPath;
            ProjectFile = projectFile.LocalPath;
            Cultures = cultureInfos ?? new[] { CultureInfo.CurrentCulture };
        }

        /// <summary>
        /// Generate or update the .incaloc.csv file specified by the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void Generate(params IncaLocParameters[] parameters)
        {
            throw new NotImplementedException();

            //foreach (var parameter in parameters)
            //{
            //    var filePath = Path.Combine(StoreLocation, parameter.FileName());

            //    Directory.CreateDirectory(StoreLocation);

            //    var csvDocuemnt = File.Exists(filePath)
            //        ? UpdateCsv(parameter, Cultures, filePath).Value()
            //        : CreateCsv(parameter, Cultures);
                
            //    csvDocuemnt.Save(filePath);

            //    IncaLocalizeResources.EmbedFile(ProjectFile, filePath).Value();
            //}
        }

        static Exception<StringBuilder> UpdateCsv(IncaLocParameters parameters, IEnumerable<CultureInfo> cultures, string filePath) 
        {
            try
            {
                var text = File.ReadAllText(filePath);
                var csv = new StringBuilder(text);

                return csv;
            }
            catch (Exception e)
            {
                return e.ToException<StringBuilder>();
            }
        }

        static StringBuilder CreateCsv(IncaLocParameters parameters, IEnumerable<CultureInfo> cultures) => new StringBuilder();

    }

}