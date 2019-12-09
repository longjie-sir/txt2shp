using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilForBasicInfo.ClassFolder;

namespace readData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(40); //设置点击时的响应时间
            //按文件
            if (this.ckFile.Checked == true)
            {
                System.Windows.Forms.FolderBrowserDialog openDia = new FolderBrowserDialog();
                if (openDia.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.Empty.Equals(openDia.SelectedPath))
                {
                    txtPath.Text = openDia.SelectedPath;
                }
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog openDia = new FolderBrowserDialog();
                if (openDia.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.Empty.Equals(openDia.SelectedPath))
                {
                    txtPath.Text = openDia.SelectedPath;
                }
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(40); //设置点击时的响应时间
            System.Windows.Forms.SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Title = "导出路径";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.Empty.Equals(saveDia.FileName))
            {
                txtOutPath.Text = saveDia.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == null || txtPath.Text == "")
            {
                MessageBox.Show("请先选择操作源!");
                return;
            }
            if (txtOutPath.Text == null || txtOutPath.Text == "")
            {
                MessageBox.Show("请选择导出的文件路径!");
                return;
            }
            //shp转txt
            if (this.chkTxt.Checked == true)
            {
                System.IO.DirectoryInfo dir = new DirectoryInfo(@txtPath.Text);
                if (dir.Exists)
                {

                    int fielCount = dir.GetFiles().Length;
                  
                    for (int x = 0; x < dir.GetFiles().Length; x++)
                    {
                        String filePath = @txtPath.Text + "\\" + dir.GetFiles()[x].ToString();
                        String outPutPath = txtOutPath.Text;
                        String extension = System.IO.Path.GetExtension(filePath);
                        if (extension.Contains("shp"))
                        {
                            writeTxt(filePath, outPutPath);
                        }
                        lbCount.Text = x + "/" + fielCount;
                    }

                }
                else
                {
                    MessageBox.Show("目录不存在,请重新检查目录路径是否正确");
                }
            }
            //按带号生成文件
            else if (this.ckFile.Checked == true)
            {
                List<Dictionary<IPolygon, List<String>>> threeFiveList = new List<Dictionary<IPolygon, List<String>>>();
                List<Dictionary<IPolygon, List<String>>> threeSixList = new List<Dictionary<IPolygon, List<String>>>();
                System.IO.DirectoryInfo dir = new DirectoryInfo(@txtPath.Text);
                if (dir.Exists)
                {
                    int fielCount = dir.GetFiles().Length;

                    for (int x = 0; x < dir.GetFiles().Length; x++)
                    {
                        String filePath = @txtPath.Text + "\\" + dir.GetFiles()[x].ToString();
                        string extension = System.IO.Path.GetExtension(filePath);
                        if (extension.Equals(".txt"))
                        {
                            String outPutPath = txtOutPath.Text;
                            readFileMerga(filePath, outPutPath, threeFiveList, threeSixList, txtLog);
                        }
                        lbCount.Text = x + "/" + fielCount;
                    }
                    List<String> filedInfo = new List<String>();
                    filedInfo.Add("点位数量");
                    filedInfo.Add("地块面积");
                    filedInfo.Add("地块编号");
                    filedInfo.Add("地块名称");
                    filedInfo.Add("图形属性");
                    filedInfo.Add("图幅号");
                    filedInfo.Add("地块用途");
                    filedInfo.Add("地类编码");

                    //分别生成两个featureClass
                    string localFilePath = System.IO.Path.GetDirectoryName(@txtOutPath.Text); //获得文件路径
                    IFeatureClass ifc = CreateMDBFeatureClass("35D", localFilePath, "35D", 35);//第一个参数是shp的名称，第二个参数是目录，第三个参数是生成存放shp的文件夹
                    IFeatureClass ifc2 = CreateMDBFeatureClass("36D", localFilePath, "36D", 36);//第一个参数是shp的名称，第二个参数是目录，第三个参数是生成存放shp的文件夹

                    for (int i = 0; i < threeFiveList.Count; i++)
                    {
                        foreach (KeyValuePair<IPolygon, List<String>> item in threeFiveList[i])
                        {
                            PolygonToClass(ifc, item.Key, item.Value, filedInfo);
                        }
                    }

                    for (int i = 0; i < threeSixList.Count; i++)
                    {
                        foreach (KeyValuePair<IPolygon, List<String>> item in threeSixList[i])
                        {
                            PolygonToClass(ifc2, item.Key, item.Value, filedInfo);
                        }
                    }
                    if (logTxt.Text.Contains(".txt"))
                    {
                        System.IO.FileStream fs = new System.IO.FileStream(@logTxt.Text, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(txtLog.Text);//开始写入值
                        sw.Close();
                        fs.Close();
                    }
                    Marshal.ReleaseComObject(ifc);
                    Marshal.ReleaseComObject(ifc2);
                }
                else
                {
                    MessageBox.Show("目录不存在,请重新检查目录路径是否正确");
                }
              
            }
            //生成单一的文件
            else if (this.ckDir.Checked == true)
            {
                System.IO.DirectoryInfo dir = new DirectoryInfo(@txtPath.Text);
                if (dir.Exists)
                {
                    int fielCount = dir.GetFiles().Length;
                    for (int x = 0; x < dir.GetFiles().Length; x++)
                    {
                        string filePath = @txtPath.Text + "\\" + dir.GetFiles()[x].ToString();
                        try 
                        {
                            string extension = System.IO.Path.GetExtension(filePath);
                            if (extension.Equals(".txt"))
                            {
                                String outPutPath = txtOutPath.Text;
                                readFile(filePath, outPutPath, txtLog);
                            }
                        }
                        catch(Exception ex)
                        {
                            txtLog.AppendText("\r\n文件：" + filePath + "发生错误：" + ex.Message);
                        }
                        lbCount.Text = x + "/" + fielCount;
                    }
                    if (logTxt.Text.Contains(".txt"))
                    {
                        System.IO.FileStream fs = new System.IO.FileStream(@logTxt.Text, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(txtLog.Text);//开始写入值
                        sw.Close();
                        fs.Close();
                    }
                    MessageBox.Show("所有文件执行完成");
                }
                else
                {
                    MessageBox.Show("目录不存在,请重新检查目录路径是否正确");
                }
            }
           
        }

        //shp写成txt
        public static void writeTxt(String shpPath,String txtPath) 
        {
            string shpfileName = System.IO.Path.GetFileName(@shpPath);
            string shpfilePath = System.IO.Path.GetDirectoryName(@shpPath);
            
            IWorkspaceFactory workspaceFC = new ShapefileWorkspaceFactory();
            IWorkspace workSpace = workspaceFC.OpenFromFile(shpfilePath, 0);
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)workSpace;
            IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(shpfileName);

            // 字段集合
            IFields pFields = pFeatureClass.Fields;
            int fieldsCount = pFields.FieldCount;

            string localFilePath = System.IO.Path.GetDirectoryName(txtPath); //获得文件路径

            string outFilePath = System.IO.Path.GetFileName(shpPath); //获得文件路径
            string fileNameExt = outFilePath.Substring(0, outFilePath.LastIndexOf(".")); //获取文件名，不带路径
            String outPath = localFilePath + "\\" + fileNameExt + ".txt";
            System.IO.FileStream fs = new System.IO.FileStream(@outPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            //开始写入
            sw.Write("[属性描述]" + "\r\n");
            sw.Write("投影类型=高斯克吕格" + "\r\n");
            sw.Write("坐标系=2000国家大地坐标系" + "\r\n");
            sw.Write("分带=3度带" + "\r\n");
            sw.Write("带号=" + "\r\n");
            sw.Write("[地块坐标]" + "\r\n");
            sw.Flush();
          
            // 要素游标
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, true);
            IFeature pFeature = pFeatureCursor.NextFeature();
           //方法中不讲ring的最后一个点（终点即起点）,在写入，因此对于每一个ring需要减去一个点的个数
            while (pFeature != null)
            {
                String arr = null;
                String pointStr = "";
                int pointNum = 0;//用于标识需要减去的点数量

                IGeometry geometry = pFeature.Shape;
                IPolygon4 polygon = new PolygonClass();
                polygon = geometry as IPolygon4;
                IPointCollection PointCol = polygon as IPointCollection;

                for (int i = 0; i < fieldsCount; i++)
                {
                    String name = pFields.get_Field(i).Name;
                    String value = "";
                    value = pFeature.get_Value(i).ToString();
                    if (name.Equals("地块面积") || name.Equals("点位数量") || name.Equals("地块编号") || name.Equals("地块名称")
                        || name.Equals("图形属性") || name.Equals("图幅号") || name.Equals("地块用途") || name.Equals("地类编码"))
                    {
                        arr += value + ",";
                    }
                }
                arr += "@";

                IGeometryBag bag = polygon.ExteriorRingBag; //获取多边形的所有外环
                IEnumGeometry geo = bag as IEnumGeometry;
                geo.Reset();
                IRing exRing = geo.Next() as IRing;
                int num = 1; //用于标识总点个数
                while (exRing != null)
                {
                    IGeometryBag bags = polygon.get_InteriorRingBag(exRing);   //获取当前外环所包含的内环
                    IEnumGeometry geos = bags as IEnumGeometry;
                    geos.Reset();
                    IRing inRing = geos.Next() as IRing;
                    int inRingNum = 2; //用于标识内环
                    while (inRing != null)   //处理内环
                    {
                        IPointCollection inRingPoint = inRing as IPointCollection;
                        for (int i = 0; i < inRingPoint.PointCount - 1; i++)
                        {
                            IPoint inPoint = inRingPoint.get_Point(i);
                            double x = inPoint.X;
                            double y = inPoint.Y;
                            x = Math.Round(x, 6);
                            y = Math.Round(y, 6);
                            pointStr += "J" + num + "," + inRingNum + "," + y + "," + x + "\r\n";
                            num++;
                            pointNum++;
                        }
                       
                        inRingNum++;
                        inRing = geos.Next() as IRing; ;
                    }

                    IPointCollection exRingPoint = exRing as IPointCollection;
                    for (int i = 0; i < exRingPoint.PointCount - 1; i++)  //处理外环
                    {
                        IPoint exPoint = exRingPoint.get_Point(i);
                        double x = exPoint.X;
                        double y = exPoint.Y;
                        x = Math.Round(x, 6);
                        y = Math.Round(y, 6);
                        pointStr += "J" + num + ",1," + y + "," + x + "\r\n";
                        num++;
                        pointNum++;
                    }
                    exRing = geo.Next() as IRing;
                  
                }
                pFeature = pFeatureCursor.NextFeature();

                arr = pointNum + "," + arr ;
                sw.Write(arr.Replace(" ","") + "\r\n");
                sw.Write(pointStr);
                //清空缓冲区
                sw.Flush();
            }
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            // 释放游标
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
        }

        //针对整个文件夹按带号整合
        public static void readFileMerga(String filePath, String outPath,List<Dictionary<IPolygon, List<String>>> threeFiveList,List<Dictionary<IPolygon, List<String>>> threeSixList,TextBox txtLog)
        {
            List<String> filedInfo = new List<String>();
            filedInfo.Add("点位数量");
            filedInfo.Add("地块面积");
            filedInfo.Add("地块编号");
            filedInfo.Add("地块名称");
            filedInfo.Add("图形属性");
            filedInfo.Add("图幅号");
            filedInfo.Add("地块用途");
            filedInfo.Add("地类编码");

            IdentifyEncoding ide = new IdentifyEncoding();
            string encod = ide.GetEncodingName(new System.IO.FileInfo(@filePath));
            Encoding encoding = ide.GetEncoding(encod); //Encoding.ASCII;//  
            System.IO.FileStream fs = new System.IO.FileStream(@filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            Dictionary<IPolygon, List<String>> geoList = new Dictionary<IPolygon, List<String>>();
            StreamReader sr = new StreamReader(fs, encoding);
            //记录每次读取的一行记录  
            string strLine = "";
            List<String> buffer = new List<String>();
            Boolean startFlag = false;
            Boolean code = false;
            String symbol = "";
            Dictionary<int, List<String>> bufferDic = new Dictionary<int, List<String>>();
            List<String> zeroBuffer = new List<String>();
            //逐行读取数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                if (strLine.IndexOf("@") > 0)
                {
                    if (buffer.Count() > 0)
                    {
                        if (buffer.Count < 3)
                        {
                            //写警告  
                            txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                        }
                        else 
                        {
                            List<String> filed = new List<String>();
                            //应该在这一步完成 面的interaction
                            IPolygon iPolygon = convert(buffer, filed);
                            if (zeroBuffer.Count > 3)
                            {
                                txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                                IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                                ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                IGeometryCollection geoPolygon = new PolygonClass();
                                geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                                iPolygon = geoPolygon as IPolygon;
                            }
                            List<IPolygon> tempInteratorList = new List<IPolygon>();
                            if (bufferDic.Count > 0)
                            {
                                foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                                {
                                    IPolygon interatorPolygon = interactorConvert(kvp.Value);
                                    tempInteratorList.Add(interatorPolygon);
                                }
                            }

                            if (tempInteratorList.Count > 0)
                            {
                                foreach (IPolygon polygon in tempInteratorList)
                                {
                                    ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                    IGeometryCollection geoPolygon = new PolygonClass();
                                    geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                                    iPolygon = geoPolygon as IPolygon;
                                }
                            }

                            if (!iPolygon.IsEmpty)
                            {
                                geoList.Add(iPolygon, filed);
                            }
                            else
                            {
                                //写警告  
                                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:");
                                foreach (String filedStr in filed)
                                {
                                    txtLog.AppendText(filedStr + ",");
                                }
                                txtLog.AppendText("@" + "\r\n");
                            }
                        }
                        //置空
                        zeroBuffer.Clear();
                        buffer.Clear();
                        bufferDic.Clear();
                    }
                    else
                    {
                        startFlag = true;
                    }
                }
                if (startFlag)
                {

                    if (buffer.Count == 0)
                    {
                        buffer.Add(strLine);
                        zeroBuffer.Add(strLine);
                        symbol = strLine;
                    }
                    if (buffer.Count > 0)
                    {
                        if (strLine.IndexOf(",1,") > 0)
                        {
                            buffer.Add(strLine);
                        }
                        else if (strLine.IndexOf(",0,") > 0)
                        {
                            zeroBuffer.Add(strLine);
                        }
                        else
                        {
                            if (strLine.IndexOf("@") < 0)
                            {
                                String[] s = strLine.Split(',');
                                String str = s[1];
                                int symbolNum = Int32.Parse(str);
                                if (!bufferDic.ContainsKey(symbolNum))
                                {
                                    List<String> tempBuffer = new List<string>();
                                    tempBuffer.Add(symbol);
                                    tempBuffer.Add(strLine);
                                    bufferDic.Add(symbolNum, tempBuffer);
                                }
                                else
                                {
                                    bufferDic[symbolNum].Add(strLine);
                                }
                            }
                        }
                    }

                }
            }
            if (buffer.Count < 3)
            {
                //写警告  
                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
            }
            else 
            {
                List<String> filedList = new List<String>();
                IPolygon lastPolygon = convert(buffer, filedList);
                IPoint point = lastPolygon.ToPoint;
                if (point.X < 36000000 && point.X > 35000000)
                {
                    code = true;
                }

                if (zeroBuffer.Count > 3)
                {
                    txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                    IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                    ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                    IGeometryCollection geoPolygon = new PolygonClass();
                    geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                    lastPolygon = geoPolygon as IPolygon;
                }

                List<IPolygon> tempList = new List<IPolygon>();

                if (bufferDic.Count > 0)
                {
                    foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                    {
                        IPolygon interatorPolygon = interactorConvert(kvp.Value);
                        tempList.Add(interatorPolygon);
                    }
                }
                try{
                     if (tempList.Count > 0)
                    {
                        foreach (IPolygon polygon in tempList)
                        {
                            ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                            IGeometryCollection geoPolygon = new PolygonClass();
                            geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                            lastPolygon = geoPolygon as IPolygon;
                        }
                    }
                    if (!lastPolygon.IsEmpty)
                    {
                        geoList.Add(lastPolygon, filedList);
                    }
                    else
                    {
                        txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                    }

                    if (code == true)
                    {
                        threeFiveList.Add(geoList);
                    }
                    else
                    {
                        threeSixList.Add(geoList);
                    }
                }catch(Exception e){
                    txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                }
               
            }
            
            sr.Close();
            fs.Close();
        }

        //针对整个文件夹
        public static void readFile(String filePath,String outPath,TextBox txtLog) 
        {
            List<String> filedInfo = new List<String>();
            filedInfo.Add("点位数量");
            filedInfo.Add("地块面积");
            filedInfo.Add("地块编号");
            filedInfo.Add("地块名称");
            filedInfo.Add("图形属性");
            filedInfo.Add("图幅号");
            filedInfo.Add("地块用途");
            filedInfo.Add("地类编码");

            IdentifyEncoding ide = new IdentifyEncoding();
            string encod = ide.GetEncodingName(new System.IO.FileInfo(@filePath));
            Encoding encoding = ide.GetEncoding(encod); //Encoding.ASCII;//  
            System.IO.FileStream fs = new System.IO.FileStream(@filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            Dictionary<IPolygon, List<String>> geoList = new Dictionary<IPolygon, List<String>>();
            StreamReader sr = new StreamReader(fs, encoding);
            //记录每次读取的一行记录  
            string strLine = "";
            List<String> buffer = new List<String>();
            List<String> zeroBuffer = new List<String>();

            String symbol = "";
            Dictionary<int, List<String>> bufferDic = new Dictionary<int, List<String>>();

            Boolean startFlag = false;
            int code = 0;
            //逐行读取数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                if (strLine.IndexOf("@") > 0)
                {
                    if (buffer.Count() > 0)
                    {
                        if (buffer.Count < 3)
                        {
                            //写警告  
                            txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                        }
                        else 
                        {
                            List<String> filed = new List<String>();
                            //应该在这一步完成 面的interaction
                            IPolygon iPolygon = convert(buffer, filed);
                            if (zeroBuffer.Count > 3)
                            {
                                txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                                IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                                ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                IGeometryCollection geoPolygon = new PolygonClass();
                                geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                                iPolygon = geoPolygon as IPolygon;
                            }
                            List<IPolygon> tempInteratorList = new List<IPolygon>();
                            if (bufferDic.Count > 0)
                            {
                                foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                                {
                                    IPolygon interatorPolygon = interactorConvert(kvp.Value);
                                    tempInteratorList.Add(interatorPolygon);
                                }
                            }

                            if (tempInteratorList.Count > 0)
                            {
                                foreach (IPolygon polygon in tempInteratorList)
                                {
                                    ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                    IGeometryCollection geoPolygon = new PolygonClass();
                                    geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                                    iPolygon = geoPolygon as IPolygon;
                                }
                            }

                            if (!iPolygon.IsEmpty)
                            {
                                geoList.Add(iPolygon, filed);
                            }
                            else
                            {
                                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:");
                                foreach (String filedStr in filed)
                                {
                                    txtLog.AppendText(filedStr + ",");
                                }
                                txtLog.AppendText("@" + "\r\n");
                            }
                        }
                        //置空
                        zeroBuffer.Clear();
                        buffer.Clear();
                        bufferDic.Clear();
                    }
                    else
                    {
                        startFlag = true;
                    }
                }
                if (startFlag)
                {

                    if (buffer.Count == 0)
                    {
                        buffer.Add(strLine);
                        zeroBuffer.Add(strLine);
                        symbol = strLine;
                    }
                    if (buffer.Count > 0)
                    {
                        if (strLine.IndexOf(",1,") > 0)
                        {
                            buffer.Add(strLine);
                        }
                        else if (strLine.IndexOf(",0,") > 0)
                        {
                            zeroBuffer.Add(strLine);
                        }
                        else
                        {
                            if (strLine.IndexOf("@") < 0)
                            {
                                String[] s = strLine.Split(',');
                                String str = s[1];
                                int symbolNum = Int32.Parse(str);
                                if (!bufferDic.ContainsKey(symbolNum))
                                {
                                    List<String> tempBuffer = new List<string>();
                                    tempBuffer.Add(symbol);
                                    tempBuffer.Add(strLine);
                                    bufferDic.Add(symbolNum, tempBuffer);
                                }
                                else
                                {
                                    bufferDic[symbolNum].Add(strLine);
                                }
                            }
                        }
                    }

                }
            }

            if (buffer.Count < 3)
            {
                //写警告  
                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
            }
            else 
            {
                List<String> filedList = new List<String>();
                IPolygon lastPolygon = convert(buffer, filedList);
                IPoint point = lastPolygon.ToPoint;
                if (point.X < 36000000 && point.X > 35000000)
                {
                    code = Int32.Parse(point.X.ToString().Substring(0, 2));
                }

                if (zeroBuffer.Count > 3)
                {
                    txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                    IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                    ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                    IGeometryCollection geoPolygon = new PolygonClass();
                    geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                    lastPolygon = geoPolygon as IPolygon;
                }

                List<IPolygon> tempList = new List<IPolygon>();

                if (bufferDic.Count > 0)
                {
                    foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                    {
                        IPolygon interatorPolygon = interactorConvert(kvp.Value);
                        tempList.Add(interatorPolygon);
                    }
                }
                try
                {
                    if (tempList.Count > 0 && !lastPolygon.IsEmpty)
                    {
                        foreach (IPolygon polygon in tempList)
                        {
                            ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                            IGeometryCollection geoPolygon = new PolygonClass();
                            geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                            lastPolygon = geoPolygon as IPolygon;
                        }
                    }
                }
                catch (Exception e)
                {
                    txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                }

                if (!lastPolygon.IsEmpty)
                {
                    geoList.Add(lastPolygon, filedList);
                }
                else
                {
                    txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                }
            }
            
            string localFilePath = System.IO.Path.GetDirectoryName(outPath); //获得文件路径

            string outFilePath = System.IO.Path.GetFileName(filePath); //获得文件路径
            string fileNameExt = outFilePath.Substring(0, outFilePath.LastIndexOf(".")); //获取文件名，不带路径

            IFeatureClass ifc = CreateMDBFeatureClass(fileNameExt, localFilePath, fileNameExt, code);//第一个参数是shp的名称，第二个参数是目录，第三个参数是生成存放shp的文件夹

            foreach (KeyValuePair<IPolygon, List<String>> item in geoList)
            {
                PolygonToClass(ifc, item.Key, item.Value, filedInfo);
            }
            sr.Close();
            fs.Close();
        }

        //针对单个txt
        public static void readFilePb(String filePath, String outPath,TextBox txtLog)
        {
            List<String> filedInfo = new List<String>();
            filedInfo.Add("点位数量");
            filedInfo.Add("地块面积");
            filedInfo.Add("地块编号");
            filedInfo.Add("地块名称");
            filedInfo.Add("图形属性");
            filedInfo.Add("图幅号");
            filedInfo.Add("地块用途");
            filedInfo.Add("地类编码");

            IdentifyEncoding ide = new IdentifyEncoding();
            string encod = ide.GetEncodingName(new System.IO.FileInfo(@filePath));
            Encoding encoding = ide.GetEncoding(encod); //Encoding.ASCII;//  
            System.IO.FileStream fs = new System.IO.FileStream(@filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            Dictionary<IPolygon, List<String>> geoList = new Dictionary<IPolygon, List<String>>();
            StreamReader sr = new StreamReader(fs, encoding);
            //记录每次读取的一行记录  
            string strLine = "";
            List<String> buffer = new List<String>();
            Boolean startFlag = false;
            int code = 0;
            List<String> zeroBuffer = new List<String>();
            String symbol = "";
            Dictionary<int, List<String>> bufferDic = new Dictionary<int, List<String>>();

            //逐行读取数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                if (strLine.IndexOf("@") > 0)
                {
                    if (buffer.Count() > 0)
                    {
                        if (buffer.Count < 3)
                        {
                            //写警告  
                            txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
                        }
                        else 
                        {
                            List<String> filed = new List<String>();
                            //应该在这一步完成 面的interaction
                            IPolygon iPolygon = convert(buffer, filed);
                            if (zeroBuffer.Count > 3)
                            {
                                txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                                IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                                ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                IGeometryCollection geoPolygon = new PolygonClass();
                                geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                                iPolygon = geoPolygon as IPolygon;
                            }
                            List<IPolygon> tempInteratorList = new List<IPolygon>();
                            if (bufferDic.Count > 0)
                            {
                                foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                                {
                                    IPolygon interatorPolygon = interactorConvert(kvp.Value);
                                    tempInteratorList.Add(interatorPolygon);
                                }
                            }

                            if (tempInteratorList.Count > 0)
                            {
                                foreach (IPolygon polygon in tempInteratorList)
                                {
                                    ITopologicalOperator unionedPolygon = iPolygon as ITopologicalOperator;
                                    IGeometryCollection geoPolygon = new PolygonClass();
                                    geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                                    iPolygon = geoPolygon as IPolygon;
                                }
                            }

                            if (!iPolygon.IsEmpty)
                            {
                                geoList.Add(iPolygon, filed);
                            }
                            else
                            {
                                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:");
                                foreach (String filedStr in filed)
                                {
                                    txtLog.AppendText(filedStr + ",");
                                }
                                txtLog.AppendText("@" + "\r\n");
                            }
                        }
                        //置空
                        zeroBuffer.Clear();
                        buffer.Clear();
                        bufferDic.Clear();
                    }
                    else
                    {
                        startFlag = true;
                    }
                }
                if (startFlag)
                {

                    if (buffer.Count == 0)
                    {
                        buffer.Add(strLine);
                        zeroBuffer.Add(strLine);
                        symbol = strLine;
                    }
                    if (buffer.Count > 0)
                    {
                        if (strLine.IndexOf(",1,") > 0)
                        {
                            buffer.Add(strLine);
                        }
                        else if (strLine.IndexOf(",0,") > 0)
                        {
                            zeroBuffer.Add(strLine);
                        }
                        else
                        {
                            if (strLine.IndexOf("@") < 0)
                            {
                                String[] s = strLine.Split(',');
                                String str = s[1];
                                int symbolNum = Int32.Parse(str);
                                if (!bufferDic.ContainsKey(symbolNum))
                                {
                                    List<String> tempBuffer = new List<string>();
                                    tempBuffer.Add(symbol);
                                    tempBuffer.Add(strLine);
                                    bufferDic.Add(symbolNum, tempBuffer);
                                }
                                else
                                {
                                    bufferDic[symbolNum].Add(strLine);
                                }
                            }
                        }
                    }
                }
            }
            if (buffer.Count < 3)
            {
                //写警告  
                txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:" + buffer[0] + "\r\n");
            }
            else 
            {
                List<String> filedList = new List<String>();
                IPolygon lastPolygon = convert(buffer, filedList);
                IPoint point = lastPolygon.ToPoint;
                if (point.X < 36000000 && point.X > 35000000)
                {
                    code = Int32.Parse(point.X.ToString().Substring(0,2));
                }

                if (zeroBuffer.Count > 3)
                {
                    txtLog.AppendText("警告：存在‘,0,’标识，当前执行的txt为:" + filePath + ",条目为:" + buffer[0] + "\r\n");
                    IPolygon zeroPolygon = interactorConvert(zeroBuffer);
                    ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                    IGeometryCollection geoPolygon = new PolygonClass();
                    geoPolygon.AddGeometryCollection(unionedPolygon.Union(zeroPolygon) as IGeometryCollection);
                    lastPolygon = geoPolygon as IPolygon;
                }

                List<IPolygon> tempList = new List<IPolygon>();

                if (bufferDic.Count > 0)
                {
                    foreach (KeyValuePair<int, List<String>> kvp in bufferDic)
                    {
                        IPolygon interatorPolygon = interactorConvert(kvp.Value);
                        tempList.Add(interatorPolygon);
                    }
                }

                if (tempList.Count > 0)
                {
                    foreach (IPolygon polygon in tempList)
                    {
                        ITopologicalOperator unionedPolygon = lastPolygon as ITopologicalOperator;
                        IGeometryCollection geoPolygon = new PolygonClass();
                        geoPolygon.AddGeometryCollection(unionedPolygon.Difference(polygon) as IGeometryCollection);
                        lastPolygon = geoPolygon as IPolygon;
                    }
                }
                if (!lastPolygon.IsEmpty)
                {
                    geoList.Add(lastPolygon, filedList);
                }
                else
                {
                    txtLog.AppendText("错误：存在面为零的情况，当前执行的txt为:" + filePath + ",错误的条目为:");
                    foreach (String filedStr in filedList)
                    {
                        txtLog.AppendText(filedStr + ",");
                    }
                    txtLog.AppendText("@" + "\r\n");
                }
            }
            string localFilePath = System.IO.Path.GetDirectoryName(outPath); //获得文件路径

            string outFilePath = System.IO.Path.GetFileName(filePath); //获得文件路径
            string fileNameExt = outFilePath.Substring(0, outFilePath.LastIndexOf(".")); //获取文件名，不带路径

            IFeatureClass ifc = CreateMDBFeatureClass(fileNameExt, localFilePath, fileNameExt,code);//第一个参数是shp的名称，第二个参数是目录，第三个参数是生成存放shp的文件夹
            foreach (KeyValuePair<IPolygon, List<String>> item in geoList)
            {
                PolygonToClass(ifc, item.Key, item.Value, filedInfo);
            }
            sr.Close();
            fs.Close();
        }

        private static IPolygon interactorConvert(List<String> buffer)
        {
            IPointCollection iPointCollection = new PolygonClass();
            Ring ring = new RingClass();
            object missing = Type.Missing;
            for (int i = 0; i < buffer.Count(); i++)
            {
                if (buffer[i].IndexOf("@") > 0)
                {
                    continue;
                }
                String[] split = buffer[i].Split(',');
                double pointX = Double.Parse(split[3]);
                double pointY = Double.Parse(split[2]);
                IPoint inPoint = new PointClass();
                inPoint.PutCoords(pointX, pointY);
                iPointCollection.AddPoint(inPoint);
            }
            ring.AddPointCollection(iPointCollection);
            IGeometryCollection pointPolygon = new PolygonClass();
            pointPolygon.AddGeometry(ring as IGeometry, ref missing, ref missing);
            IPolygon polyGonGeo = pointPolygon as IPolygon;
            polyGonGeo.SimplifyPreserveFromTo();
            return polyGonGeo;
        }

        //将点转换为Polygon
        private static IPolygon convert(List<String> buffer, List<String> filedList)
        {
            IPointCollection iPointCollection = new PolygonClass();
            Ring ring = new RingClass();
            object missing = Type.Missing;
            for (int i = 0; i < buffer.Count(); i++)
            {
                if (buffer[i].IndexOf("@") > 0)
                {
                    filedList.Clear();
                    String[] str = buffer[i].Split(',');
                    for (int a = 0; a < str.Length - 1; a++)
                    {
                        filedList.Add(str[a]);
                    }
                    continue;
                }
                String[] split = buffer[i].Split(',');
                double pointX = Double.Parse(split[3]);
                double pointY = Double.Parse(split[2]);
                IPoint inPoint = new PointClass();
                inPoint.PutCoords(pointX, pointY);
                iPointCollection.AddPoint(inPoint);
            }
            ring.AddPointCollection(iPointCollection);
            IGeometryCollection pointPolygon = new PolygonClass();
            pointPolygon.AddGeometry(ring as IGeometry, ref missing, ref missing);
            IPolygon polyGonGeo = pointPolygon as IPolygon;
            polyGonGeo.SimplifyPreserveFromTo();
            return polyGonGeo;
        }

        //建立FeatureClass
        public static IFeatureClass CreateMDBFeatureClass(String FeatureClassName, String shpFullFilePath, String shpFileName, int code)
        {
            if (Directory.Exists(shpFullFilePath + "\\" + shpFileName) == true)//如果存在就删除文件夹
            {
                Directory.Delete(shpFullFilePath + "\\" + shpFileName, true);
            }

            IWorkspaceFactory pWorkspaceFac = new ShapefileWorkspaceFactoryClass();
            IWorkspaceName workspaceName = pWorkspaceFac.Create(shpFullFilePath, shpFileName, null, 0);
            IFeatureWorkspace featureWorkspace = (workspaceName as IName).Open() as IFeatureWorkspace;
            IFeatureClassDescription fcDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDesc = (IObjectClassDescription)fcDesc;
            //创建字段集2
            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;//创建必要字段
            IFields fields = ocDescription.RequiredFields;
            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
            IFieldsEdit pFieldsEdit = (IFieldsEdit)fields;
            AddField(pFieldsEdit);
            IField Ifield = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = Ifield.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
            ISpatialReferenceFactory pSpatialRefFac = new SpatialReferenceEnvironmentClass();
            int pro = 4488 + code;
          
            IProjectedCoordinateSystem ipro = pSpatialRefFac.CreateProjectedCoordinateSystem(pro);
            geometryDefEdit.SpatialReference_2 = ipro;

            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = fields; //将传入字段 转成 validatedFields
            fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
            IFeatureClass ifc = featureWorkspace.CreateFeatureClass(FeatureClassName, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, esriFeatureType.esriFTSimple, "Shape", "");
            IWorkspaceFactoryLockControl fl = (IWorkspaceFactoryLockControl)pWorkspaceFac;
            if (fl.SchemaLockingEnabled)
            {
                fl.DisableSchemaLocking();
            }
            return ifc;
        }

        //把Polygon写入FeatureClass
        public static void PolygonToClass(IFeatureClass pFeatureClass, IPolygon pPlolygon, List<String> filedList, List<String> filedInfo)
        {
            IFeature feature = pFeatureClass.CreateFeature();
            feature.Shape = pPlolygon;
            for (int i = 0; i < filedList.Count; i++)
            {
                int dataName = pFeatureClass.FindField(filedInfo[i]);
                if (filedInfo[i].Equals("地块面积"))
                {
                    if (filedList[i] != null && filedList[i] != "" && !filedList[i].Equals("") && filedList[i].Length != 0)
                    {
                        feature.set_Value(dataName, Double.Parse(filedList[i]));
                    }
                    else 
                    {
                        feature.set_Value(dataName, 0.0);
                    }
                   
                }
                else if (filedInfo[i].Equals("点位数量"))
                {
                    if (filedList[i] != null && filedList[i] != "" && !filedList[i].Equals("") && filedList[i].Length != 0)
                    {
                        feature.set_Value(dataName, Int32.Parse(filedList[i]));
                    }
                    else
                    {
                        feature.set_Value(dataName, 0);
                    }

                }
                else
                {
                    feature.set_Value(dataName, filedList[i]);
                }

            }
            feature.Store();
        }

        private void ckFile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckFile.Checked == true)
            {
                this.txtPath.Text = "";
                this.ckDir.Checked = false;
                this.chkTxt.Checked = false;
                this.btnInput.Enabled = true;
                this.txtPath.Enabled = true;
                this.txtPath.Text = "";
            }
            if (this.ckFile.Checked == false && this.ckDir.Checked == false && this.chkTxt.Checked == false)
            {
                txtPath.Text = "";
                this.btnInput.Enabled = false;
                this.txtPath.Enabled = false;
            }
        }

        private void ckDir_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckDir.Checked == true)
            {
                this.txtPath.Text = "";
                this.ckFile.Checked = false;
                this.chkTxt.Checked = false;
                this.btnInput.Enabled = true;
                this.txtPath.Enabled = true;
            }
            if (this.ckFile.Checked == false && this.ckDir.Checked == false && this.chkTxt.Checked == false)
            {
                txtPath.Text = "";
                this.btnInput.Enabled = false;
                this.txtPath.Enabled = false;
            }
        }

        //填加字段
        public static void AddField(IFieldsEdit xFieldsEdit)
        {
            IField xField = new FieldClass();
            IFieldEdit xFileEdit = (IFieldEdit)xField;
            xFileEdit.Name_2 = "点位数量";
            xFileEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            xFileEdit.IsNullable_2 = true;
            xFieldsEdit.AddField(xField);

            IField xField2 = new FieldClass();
            IFieldEdit xFileEdit2 = (IFieldEdit)xField2;
            xFileEdit2.Name_2 = "地块面积";
            xFileEdit2.Type_2 = esriFieldType.esriFieldTypeDouble;
            xFileEdit2.IsNullable_2 = true;
            xFieldsEdit.AddField(xField2);

            IField xField3 = new FieldClass();
            IFieldEdit xFileEdit3 = (IFieldEdit)xField3;
            xFileEdit3.Name_2 = "地块编号";
            xFileEdit3.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit3.Length_2 = 50;
            xFileEdit3.IsNullable_2 = true;
            xFieldsEdit.AddField(xField3);

            IField xField4 = new FieldClass();
            IFieldEdit xFileEdit4 = (IFieldEdit)xField4;
            xFileEdit4.Name_2 = "地块名称";
            xFileEdit4.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit4.Length_2 = 50;
            xFileEdit4.IsNullable_2 = true;
            xFieldsEdit.AddField(xField4);

            IField xField5 = new FieldClass();
            IFieldEdit xFileEdit5 = (IFieldEdit)xField5;
            xFileEdit5.Name_2 = "图形属性";
            xFileEdit5.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit5.Length_2 = 50;
            xFileEdit5.IsNullable_2 = true;
            xFieldsEdit.AddField(xField5);

            IField xField6 = new FieldClass();
            IFieldEdit xFileEdit6 = (IFieldEdit)xField6;
            xFileEdit6.Name_2 = "图幅号";
            xFileEdit6.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit6.Length_2 = 20;
            xFileEdit6.IsNullable_2 = true;
            xFieldsEdit.AddField(xField6);

            IField xField7 = new FieldClass();
            IFieldEdit xFileEdit7 = (IFieldEdit)xField7;
            xFileEdit7.Name_2 = "地块用途";
            xFileEdit7.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit7.Length_2 = 30;
            xFileEdit7.IsNullable_2 = true;
            xFieldsEdit.AddField(xField7);

            IField xField8 = new FieldClass();
            IFieldEdit xFileEdit8 = (IFieldEdit)xField8;
            xFileEdit8.Name_2 = "地类编码";
            xFileEdit8.Type_2 = esriFieldType.esriFieldTypeString;
            xFileEdit8.Length_2 = 30;
            xFileEdit8.IsNullable_2 = true;
            xFieldsEdit.AddField(xField8);

        }

        private void chkTxt_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTxt.Checked == true)
            {
                this.txtPath.Text = "";
                this.ckDir.Checked = false;
                this.ckFile.Checked = false;
                this.btnInput.Enabled = true;
                this.txtPath.Enabled = true;
                this.txtPath.Text = "";
            }
            if (this.ckFile.Checked == false && this.ckDir.Checked == false && this.chkTxt.Checked == false)
            {
                txtPath.Text = "";
                this.btnInput.Enabled = false;
                this.txtPath.Enabled = false;
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "TXT文件|*.txt";
            saveDia.Title = "导出日志路径";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.Empty.Equals(saveDia.FileName))
            {
                logTxt.Text = saveDia.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath buttonPath = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Rectangle newRectangle = this.btnStart.ClientRectangle;
            newRectangle.Inflate(5, 5);

            buttonPath.AddEllipse(newRectangle);
            this.btnStart.Region = new System.Drawing.Region(buttonPath);
        }

        private void btnStart_Paint(object sender, PaintEventArgs e)
        {
       //     System.Drawing.Drawing2D.GraphicsPath buttonPath =
       //new System.Drawing.Drawing2D.GraphicsPath();

       //     // Set a new rectangle to the same size as the button's 
       //     // ClientRectangle property.
       //     System.Drawing.Rectangle newRectangle = btnStart.ClientRectangle;
            
       //     // Decrease the size of the rectangle.
       //     newRectangle.Inflate(-5, -5);

       //     // Draw the button's border.
       //     e.Graphics.DrawEllipse(System.Drawing.Pens.Black, newRectangle);

       //     // Increase the size of the rectangle to include the border.
       //     newRectangle.Inflate(0, 0);

       //     // Create a circle within the new rectangle.
       //     buttonPath.AddEllipse(newRectangle);

       //     // Set the button's Region property to the newly created 
       //     // circle region.
       //     btnStart.Region = new System.Drawing.Region(buttonPath);
            
        }

    }

    

}
