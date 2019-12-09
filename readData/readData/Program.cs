using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace readData
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            #region 获取License

            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
            IAoInitialize aoInitialize = new AoInitialize();
            licenseStatus = aoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);

            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
