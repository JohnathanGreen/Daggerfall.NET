using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Content
{
    /// <summary>
    /// A <see cref="ContentManager"/> that takes settings from a <see cref="ResourceManager"/> (from a "resource1.resx" file).
    /// </summary>
    /// <remarks>
    /// You can use "." in your resource names to act as a path separator (because '/' is disallowed), and then continue to access a subresource with "Foo/Bar".
    /// </remarks>
    class ResourceContentManager : ContentManager
    {
        /// <summary>Backing field of the <see cref="ResourceManager"/> property.</summary>
        readonly ResourceManager resourceManager;

        /// <summary>Get or set a <see cref="CultureInfo"/> to use when loading resources instead of the <see cref="CultureInfo"/> of the current <see cref="System.Threading.Thread"/>. This will not affect any assets that have already been loaded.</summary>
        public CultureInfo OverrideCulture { get; set; }

        /// <summary>Get the <see cref="System.Resources.ResourceManager"/> to use as a source of content.</summary>
        public ResourceManager ResourceManager { get { return resourceManager; } }

        /// <summary>Prepare the <see cref="ResourceContentManager"/> for use as a <see cref="ContentManager"/>.</summary>
        /// <param name="resourceManager">The <see cref="System.Resources.ResourceManager"/> to use as a source of content.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> (usually from <see cref="Game.Services"/>) to find services.</param>
        public ResourceContentManager(ResourceManager resourceManager, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.resourceManager = resourceManager;
        }

        protected override Stream OpenStream(string assetName)
        {
            assetName = assetName.Replace('/', '.').Replace('\\', '.');
            return resourceManager.GetStream(assetName);
        }
    }
}
