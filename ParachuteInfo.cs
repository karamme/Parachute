using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Parachute
{
    public class ParachuteInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Parachute";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return Properties.Resources.parachute_24;
            }
        }
        public override string Description
        {
            get
            {
                return "Display names above components";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("b0e5b0dc-2c02-48ba-bbcc-6257d22242cc");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Kai Schramme";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "hello@kaischramme.com";
            }
        }
    }
}
