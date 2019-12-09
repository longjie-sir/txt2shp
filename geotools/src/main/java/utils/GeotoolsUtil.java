package utils;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.Serializable;
import java.io.StringReader;
import java.io.StringWriter;
import java.net.MalformedURLException;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import org.geotools.data.DataUtilities;
import org.geotools.data.FeatureWriter;
import org.geotools.data.Transaction;
import org.geotools.data.collection.ListFeatureCollection;
import org.geotools.data.shapefile.ShapefileDataStore;
import org.geotools.data.shapefile.ShapefileDataStoreFactory;
import org.geotools.data.simple.SimpleFeatureCollection;
import org.geotools.data.simple.SimpleFeatureIterator;
import org.geotools.data.simple.SimpleFeatureSource;
import org.geotools.feature.FeatureCollection;
import org.geotools.feature.FeatureIterator;
import org.geotools.feature.simple.SimpleFeatureBuilder;
import org.geotools.feature.simple.SimpleFeatureTypeBuilder;
import org.geotools.geojson.feature.FeatureJSON;
import org.geotools.geojson.geom.GeometryJSON;
import org.geotools.referencing.CRS;
import org.opengis.feature.Property;
import org.opengis.feature.simple.SimpleFeature;
import org.opengis.feature.simple.SimpleFeatureType;
import org.opengis.feature.type.FeatureType;
import org.opengis.referencing.FactoryException;
import org.opengis.referencing.crs.CoordinateReferenceSystem;

import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import com.esri.core.geometry.OperatorDifference;
import com.esri.core.geometry.OperatorEquals;
import com.esri.core.geometry.OperatorExportToGeoJson;
import com.esri.core.geometry.OperatorIntersection;
import com.esri.core.geometry.OperatorIntersects;
import com.esri.core.geometry.SpatialReference;
import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.LinearRing;
import com.vividsolutions.jts.geom.MultiPolygon;
import com.vividsolutions.jts.geom.Polygon;
import com.vividsolutions.jts.geom.impl.CoordinateArraySequence;
import com.vividsolutions.jts.io.WKTReader;

public class GeotoolsUtil {
	private static GeometryFactory gf = new GeometryFactory();
	
	public static void main(String[] args) throws Exception {
		txtToShp("E:\\TXT\\CJ23242019-32.txt","E:\\TXT\\test.shp");
	}
	
	/** 
     * shp转换为Geojson 
     * @param shpPath 
     * @return 
	 * @throws Exception 
     */  
    public static String shape2Geojson(String shpPath, String jsonPath) throws Exception{  
        FeatureJSON fjson = new FeatureJSON(new GeometryJSON(15));  
        // fjson_15已经保留15位
        fjson.setEncodeFeatureCollectionCRS(true);
        fjson.setEncodeNullValues(true);
        StringBuffer sb = new StringBuffer(); 
        try{  
            sb.append("{\"type\": \"FeatureCollection\",\"features\": ");  
              
            File file = new File(shpPath);  
            ShapefileDataStore shpDataStore = null;  
              
            shpDataStore = new ShapefileDataStore(file.toURL());  
            //设置编码  
            Charset charset = Charset.forName("GBK");  
            shpDataStore.setCharset(charset);  
            String typeName = shpDataStore.getTypeNames()[0];  
            SimpleFeatureSource featureSource = null;  
            featureSource =  shpDataStore.getFeatureSource (typeName);  
            SimpleFeatureCollection result = featureSource.getFeatures();  
            SimpleFeatureIterator itertor = result.features();  
            JSONArray array = new JSONArray();  
            while (itertor.hasNext())  
            {  
                SimpleFeature feature = itertor.next();
                StringWriter writer = new StringWriter();  
                fjson.writeFeature(feature, writer);  
                JSONObject json = JSONObject.parseObject(writer.toString()); 
                array.add(json);  
            }  
            itertor.close();  
            sb.append(array.toString());  
            sb.append("}");  
            //写入文件  
            FileWriter writer;
            try {
              writer = new FileWriter(jsonPath);
              writer.write(sb.toString());
              writer.flush();
              writer.close();
            } catch (IOException e) {
              e.printStackTrace();
            }
        }  
        catch(Exception e){  
            e.printStackTrace();  
            throw new Exception(e);
        }  
        return sb.toString();
    }
    
    /**
     * 投影坐标的shp转换为地理坐标的GeoJson
     * @param shpPath shp文件路径
     * @return
     * @throws Exception 
     */
  	public static String convertShpToJson(String shpPath, String jsonPath) throws Exception{
	  FeatureJSON fjson = new FeatureJSON(new GeometryJSON(15));
	  JSONObject geojsonObject=new JSONObject();
	  geojsonObject.put("type","FeatureCollection");
	  File file = new File(shpPath);
	  ShapefileDataStore shpDataStore = null;
	  try {
		shpDataStore = new ShapefileDataStore(file.toURL());
		} catch (MalformedURLException e1) {
			e1.printStackTrace();
		}

	  String typeName;
	  try {
		  typeName = shpDataStore.getTypeNames()[0];
		  FeatureType schema =shpDataStore.getSchema(typeName);
		  CoordinateReferenceSystem srsSystem =schema.getCoordinateReferenceSystem();
		  
		  CoordinateReferenceSystem crs2000 = null;
		try {
			crs2000 = CRS.parseWKT(CRSTransform.cgcs2000);
		} catch (FactoryException e1) {
			e1.printStackTrace();
		}
		  
		  SimpleFeatureSource featureSource = null;
		  featureSource =  shpDataStore.getFeatureSource (typeName);
		  SimpleFeatureCollection result = featureSource.getFeatures();
		  SimpleFeatureIterator itertor = result.features();
		  JSONArray array = new JSONArray();
		  //生成JSON
		  while (itertor.hasNext())
		  {
		      SimpleFeature feature = itertor.next(); 
		      //将geometry进行坐标转换
			  Geometry geo = ((Geometry)feature.getDefaultGeometry());
			  geo =  CRSTransform.transPartCGCS2000CRSGeoToProject(geo, crs2000, srsSystem);//transCGCS2000CRSProjectToGeo((Geometry)feature.getDefaultGeometry());
			  feature.setDefaultGeometry(geo);
			  StringWriter writer = new StringWriter();
			  fjson.writeFeature(feature, writer);
			  String temp=writer.toString();
			  byte[] b=temp.getBytes("iso8859-1");
			  temp=new String(b,"gbk");
			  JSONObject json =  JSONObject.parseObject(temp);
		      array.add(json);
		  }
		  geojsonObject.put("features",array);
		  //写入文件  
          FileWriter writer;
          try {
            writer = new FileWriter(jsonPath);
            writer.write(geojsonObject.toString());
            writer.flush();
            writer.close();
          } catch (IOException e) {
            e.printStackTrace();
          }
		  itertor.close();
		} catch (IOException e) {
			e.printStackTrace();
			throw new Exception(e);
		}
	  return geojsonObject.toString();
  	}

  	
  	/**
     *txt文件读取，直角坐标转换为地理坐标
  	 * @param filePath  txt文件路径
  	 * @param transForm 是否需要将直角坐标转换为地理坐标
  	 * @return
  	 * @throws Exception 
  	 */
  	public static String wKTConvertor(String filePath,boolean transForm, String jsonPath) throws Exception {
          File f = new File(filePath);
          JSONObject result = new JSONObject();
          FileInputStream fis = null;
          InputStreamReader isr = null;
          BufferedReader br = null;
          List<Geometry> geoList = new ArrayList<Geometry>();
          List<ArrayList<String>> filedsList = new ArrayList<ArrayList<String>>();
          try {
              fis = new FileInputStream(f);
              isr = new InputStreamReader(fis);
              br = new BufferedReader(isr);
              String temp = null;
              ArrayList<String> buffer = new ArrayList<String>();
              boolean startFlag = false;
              
              ArrayList<String> zeroBuffer = new ArrayList<String>();
              String symbol = "";
              Map<Integer,ArrayList<String>> bufferMap = new HashMap<Integer,ArrayList<String>>(); 
              
              //逐行读取数据
              while ((temp = br.readLine()) != null) {
                  if (temp.contains("@")) {
                      if (buffer.size() > 0) {
                    	  if(buffer.size() > 3) 
                    	  {
                    		  ArrayList<String> filed = new ArrayList<String>();
                              Geometry geo = convert(buffer,filed);
                              if(zeroBuffer.size() > 3) {
                            	  Geometry zeroGeo = interactorConvert(zeroBuffer);
                            	  geo = geo.union(zeroGeo);
                              }
                              ArrayList<Geometry> tempInteratorList = new ArrayList<Geometry>();
                              if(bufferMap.size() > 0) {
                            	  for (Map.Entry<Integer, ArrayList<String>> entry : bufferMap.entrySet()) {
                            		  Geometry tempGeo = interactorConvert(entry.getValue());
                            		  tempInteratorList.add(tempGeo);
                            	  }
                              }
                              
                              if(tempInteratorList.size() > 0) {
                            	  for(Geometry tempGeo : tempInteratorList) {
                            		  if(geo.intersects(tempGeo)) {
                            			  geo = geo.difference(tempGeo);
                            		  }
                            	  }
                              }
                              if(!geo.isEmpty()) {
                            	  filedsList.add(filed);
                                  geoList.add(geo);
                              }
                              zeroBuffer.clear();
                              bufferMap.clear();
                              buffer.clear();
                    	  }
                      	 
                      } else {
                          startFlag = true;
                      }
                  }
                  if (startFlag) {
                	  
                	  if(buffer.size() == 0) {
                		  buffer.add(temp);
                		  zeroBuffer.add(temp);
                		  symbol = temp;
                	  }
                	  
                	  if(buffer.size() > 0) {
                		  if(temp.indexOf(",1,") > 0) {
                			  buffer.add(temp);
                		  }else if(temp.indexOf(",0,") > 0) {
                			  zeroBuffer.add(temp);
                		  }else {
                			  if(temp.indexOf("@") < 0) {
                				  String[] s = temp.split(",");
                				  String str = s[1];
                				  int symbolNum = Integer.parseInt(str);
                				  if(!bufferMap.containsKey(symbolNum)) {
                					  ArrayList<String> tempList = new ArrayList<String>();
                					  tempList.add(symbol);
                					  tempList.add(temp);
                					  bufferMap.put(symbolNum, tempList);
                				  }else {
                					  bufferMap.get(symbolNum).add(temp);
                				  }
                			  }
                		  }
                	  }
                  }
              }
              
              ArrayList<String> filedList = new ArrayList<String>();
              Geometry lastGeo = convert(buffer,filedList);
              
              if(zeroBuffer.size() > 3) {
            	  Geometry zeroGeo = interactorConvert(zeroBuffer);
            	  lastGeo = lastGeo.union(zeroGeo);
              }
              ArrayList<Geometry> tempInteratorList = new ArrayList<Geometry>();
              if(bufferMap.size() > 0) {
            	  for (Map.Entry<Integer, ArrayList<String>> entry : bufferMap.entrySet()) {
            		  Geometry tempGeo = interactorConvert(entry.getValue());
            		  tempInteratorList.add(tempGeo);
            	  }
              }
              
              if(tempInteratorList.size() > 0 && !lastGeo.isEmpty()) {
            	  for(Geometry tempGeo : tempInteratorList) {
            		  lastGeo = lastGeo.difference(tempGeo);
            	  }
              }
              
              if(!lastGeo.isEmpty()) {
            	  filedsList.add(filedList);
                  geoList.add(lastGeo);
              }
            
              //用wkt生成LineString
              WKTReader reader = new WKTReader( gf );
              List<String> wktList = new ArrayList<String>(); 
              for(int i = 0; i < geoList.size(); i++) {
              	Polygon polygon = (Polygon)reader.read(geoList.get(i).toString());
              	//将Polygon转换成地理坐标
              	if(transForm == true) {
              		//将CGCS2000投影坐标转换为地理坐标
              		Geometry convertedPolygon = CRSTransform.transCGCS2000CRSProjectToGeo(polygon);
                  	wktList.add(convertedPolygon.toString());
              	}else {
              		wktList.add(polygon.toString());
              	}
              }
              //用多个Polyogn 生成FeatureCollection
              result =  convertFeatureCollection(wktList,filedsList);
              //写入文件  
              FileWriter writer;
              try {
                writer = new FileWriter(jsonPath);
                writer.write(result.toString());
                writer.flush();
                writer.close();
              } catch (IOException e) {
                e.printStackTrace();
              }
          } catch (Exception e) {
              e.printStackTrace();
              throw new Exception(e);
          } finally {
              if (br != null) {
                  try {
                      br.close();
                  } catch (IOException e) {
                      // TODO Auto-generated catch block
                      e.printStackTrace();
                  }
              }
              if (isr != null) {
                  try {
                      isr.close();
                  } catch (IOException e) {
                      // TODO Auto-generated catch block
                      e.printStackTrace();
                  }
              }
              if (fis != null) {
                  try {
                      fis.close();
                  } catch (IOException e) {
                      // TODO Auto-generated catch block
                      e.printStackTrace();
                  }
              }
          }
          return result.toString();
      }
  	
  	
  	/**
  	 * read TXT的点位并生成Geometry
  	 * @param buffer  存放每一行坐标信息的list
  	 * @param filedList
  	 * @return
  	 * @throws Exception 
  	 */
    public static Geometry convert(ArrayList<String> buffer, ArrayList<String> filedList) throws Exception{
        TreeMap<Integer, ArrayList<Coordinate>> rings = new TreeMap<Integer, ArrayList<Coordinate>>();
        boolean transform = false;
        int symbol = 0;
        double startX = 0.0;
        double startY = 0.0;
        for (String x : buffer) {
            if (x.contains("@")) {
            	filedList.clear();
            	String[] str =x.split(",");
            	for (int a = 0; a < str.length - 1; a++)
                {
                    filedList.add(str[a]);
                }
                continue;
            }
            String[] split = x.split(",");
            int ringNumber = Integer.parseInt(split[1]);
            symbol = ringNumber;
            double pointX = Double.parseDouble(split[3]);
            double pointY = Double.parseDouble(split[2]);
            if (pointX > 200 && !transform) {
                transform = true;
            }
            ArrayList<Coordinate> tempArray = rings.get(ringNumber);
            if (tempArray == null) {
                tempArray = new ArrayList<Coordinate>();
                rings.put(ringNumber, tempArray);
            }
            if(startX == 0) {
            	startX= pointX;
            }
            if(startY == 0) {
            	startY= pointY;
            }
            tempArray.add(new Coordinate(pointX, pointY));
        }
        ArrayList<Coordinate> mainArray = rings.get(symbol);
        mainArray.add(mainArray.get(0));//闭合环

        LinearRing mainLR = new LinearRing(
                new CoordinateArraySequence(mainArray.toArray(new Coordinate[mainArray.size()])), gf);
        LinearRing[] holes = null;
        if (rings.size() > 1) {
            holes = new LinearRing[rings.size() - 1];
            for (int i = 2; i <= rings.size(); i++) {
                ArrayList<Coordinate> arrayList = rings.get(i);
                arrayList.add(arrayList.get(0));
                LinearRing tempLR = new LinearRing(
                        new CoordinateArraySequence(arrayList.toArray(new Coordinate[arrayList.size()])), gf);
                holes[i - 2] = tempLR;
            }
        }
        Polygon createPolygon = gf.createPolygon(mainLR, holes);
        Geometry result = (Geometry)createPolygon;//reader.read(polygonStr);
        return result;
    }
    
    
	/**
  	 * read TXT的点位并生成Geometry
  	 * @param buffer  存放每一行坐标信息的list
  	 * @param filedList
  	 * @return
	 * @throws Exception 
  	 */
    public static Geometry interactorConvert(ArrayList<String> buffer) throws Exception{
    	  TreeMap<Integer, ArrayList<Coordinate>> rings = new TreeMap<Integer, ArrayList<Coordinate>>();
          boolean transform = false;
          int symbol = 0;
          for (String x : buffer) {
              if (x.contains("@")) {
                  continue;
              }
              String[] split = x.split(",");
              int ringNumber = Integer.parseInt(split[1]);
              symbol = ringNumber;
              double pointX = Double.parseDouble(split[3]);
              double pointY = Double.parseDouble(split[2]);
              if (pointX > 200 && !transform) {
                  transform = true;
              }
              ArrayList<Coordinate> tempArray = rings.get(ringNumber);
              if (tempArray == null) {
                  tempArray = new ArrayList<Coordinate>();
                  rings.put(ringNumber, tempArray);
              }

              tempArray.add(new Coordinate(pointX, pointY));
          }
          
          ArrayList<Coordinate> mainArray = rings.get(symbol);
          mainArray.add(mainArray.get(0)); //闭合环
          LinearRing mainLR = new LinearRing(
                  new CoordinateArraySequence(mainArray.toArray(new Coordinate[mainArray.size()])), gf);
          LinearRing[] holes = null;
          if (rings.size() > 1) {
              holes = new LinearRing[rings.size() - 1];
              for (int i = 2; i <= rings.size(); i++) {
                  ArrayList<Coordinate> arrayList = rings.get(i);
                  arrayList.add(arrayList.get(0));
                  LinearRing tempLR = new LinearRing(
                          new CoordinateArraySequence(arrayList.toArray(new Coordinate[arrayList.size()])), gf);
                  holes[i - 2] = tempLR;
              }
          }
          Polygon createPolygon = gf.createPolygon(mainLR, holes);
          Geometry result = (Geometry)createPolygon;//reader.read(polygonStr);
          return result;
    }
    
    
    /**
     * wkt文件整合成FeatureCollection
     * @param WKTS
     * @param filedsList
     * @return
     * @throws Exception
     */
    public static JSONObject convertFeatureCollection(List<String> WKTS,List<ArrayList<String>> filedsList) throws Exception {
    	final SimpleFeatureType TYPE = DataUtilities.createType("Location","geom,"
    			+"pointNumb:String,"
    			+"plotArea:String,"
    			+"serial:String,"
    			+"plotName:String,"
    			+"geoType:String,"
    			+"frameNumb:String,"
    			+"use:String,"
    			+"remarks:String");
    	SimpleFeatureBuilder featureBuilder = new SimpleFeatureBuilder(TYPE);
    	GeometryFactory geometryFactory = new GeometryFactory();
    	WKTReader reader = new WKTReader( geometryFactory );
    	FeatureJSON fjson = new FeatureJSON(new GeometryJSON(15));
    	List<SimpleFeature> features = new ArrayList();
    	SimpleFeatureCollection collection = new ListFeatureCollection(TYPE, features);
    	
    	for (int x =0;x< WKTS.size();x++){
			Polygon polygon = (Polygon)reader.read(WKTS.get(x));
		    featureBuilder.add(polygon);
		    SimpleFeature feature = featureBuilder.buildFeature(null);
		    for(int i = 0; i < feature.getAttributeCount();i++) {
		    	if(i == 0 ) {
		    		feature.setAttribute(i, polygon); //设置几何属性
		    	}else {
		    		if(filedsList.get(x).size() >= i-1) {
		    			feature.setAttribute(i, filedsList.get(x).get(i-1)); //设置其他属性
		    		}else {
		    			feature.setAttribute(i, " "); //设置其他属性
		    		}
		    	}
	    	}
		    features.add(feature);
    	}
    	//System.out.println();
    	StringWriter writer = new StringWriter();
    	fjson.writeFeatureCollection(collection, writer);
    	JSONObject result = JSONObject.parseObject(writer.toString());
    	return(result);
	}


	/**
	 * 删除Map中要素与geometry相交的区域
	 * @author longjie
	 * @param iteratorLine  存放Geometry的Map
	 * @param srf   参考坐标系
	 * @return 新的存放Geometry的map
	 * @throws Exception 
	 */
	public static Map<com.esri.core.geometry.Geometry,Map<String,Object>> iteratorLine(Map<com.esri.core.geometry.Geometry,Map<String,Object>> tempLineMap,SpatialReference srf) throws Exception {
		boolean end = false;
		//保存最短线段的Map
		Map<com.esri.core.geometry.Geometry,Map<String,Object>> shortestMap = new HashMap<com.esri.core.geometry.Geometry,Map<String,Object>>();
		Map<String,Object> tempAttrMap = new HashMap<String,Object>();
		//每一次取最短线段
		while(end == false) {
			List<com.esri.core.geometry.Geometry> geoList = new ArrayList<com.esri.core.geometry.Geometry>();
			com.esri.core.geometry.Geometry shortestLine = null;
			for (Map.Entry<com.esri.core.geometry.Geometry,Map<String,Object>> entry : tempLineMap.entrySet()) {
				com.esri.core.geometry.Geometry shortLine = entry.getKey();
				if(null == shortestLine) {
					shortestLine = shortLine;
					
				}else if(OperatorEquals.local().execute(shortestLine, shortLine, srf, null)) {

					
				}else {
					if(shortestLine.calculateLength2D() > shortLine.calculateLength2D()) {
						shortestLine =  shortLine;
					}
				}
				if(tempAttrMap.isEmpty()) {
					tempAttrMap = entry.getValue();
				}
			}
			//剔除相等的线段
			for(com.esri.core.geometry.Geometry geo : geoList) {
				tempLineMap.remove(geo);
			}
			
			if(tempLineMap.isEmpty()) {
				end = true;
			}else {
				
				//剔除最短的线段
				tempLineMap.remove(shortestLine);
				
				Map<com.esri.core.geometry.Geometry,Map<String,Object>> newLineMap = new HashMap<com.esri.core.geometry.Geometry,Map<String,Object>>();
				
				boolean symbol = false;
				
				for (Map.Entry<com.esri.core.geometry.Geometry,Map<String,Object>> entry : tempLineMap.entrySet()) {
					com.esri.core.geometry.Geometry line = entry.getKey();
					//如果线段相交，则取不同的部分进行保留
					if(OperatorIntersects.local().execute(shortestLine, line, srf, null)) {	
						com.esri.core.geometry.Geometry interruptLine = OperatorDifference.local().execute(line, shortestLine, srf, null);
						String geojson = OperatorExportToGeoJson.local().execute(interruptLine);
						JSONObject jsonObj = JSONObject.parseObject(geojson);

						if(jsonObj.getString("coordinates") != "null" &&  jsonObj.getString("coordinates") != null && jsonObj.getString("coordinates").length()>6) {
							newLineMap.put(interruptLine, entry.getValue());
						}
						
						com.esri.core.geometry.Geometry IntersectLine = OperatorIntersection.local().execute(line, shortestLine, srf, null);
						String json = OperatorExportToGeoJson.local().execute(IntersectLine);
						JSONObject jObj = JSONObject.parseObject(json);
						if(jObj.getString("coordinates") != "null" &&  jObj.getString("coordinates") != null && jObj.getString("coordinates").length()>6) {
							symbol = true;
							shortestMap.put(IntersectLine, entry.getValue());
							com.esri.core.geometry.Geometry difLine = OperatorDifference.local().execute(shortestLine, IntersectLine, srf, null);
							String difJson = OperatorExportToGeoJson.local().execute(difLine);
							JSONObject difObj = JSONObject.parseObject(difJson);
							if(difObj.getString("coordinates") != "null" &&  difObj.getString("coordinates") != null && difObj.getString("coordinates").length()>6) {
								shortestMap.put(difLine, entry.getValue());
							}
						}
					//不相交则直接保留	
					}else {
						newLineMap.put(line, entry.getValue());
					}
				}
				
				if(symbol == false) {
					shortestMap.put(shortestLine, tempAttrMap);
				}
				tempLineMap = newLineMap;
			}
		}
		
		for (Map.Entry<com.esri.core.geometry.Geometry,Map<String,Object>> entry : tempLineMap.entrySet()) {
			shortestMap.put(entry.getKey(), tempAttrMap);
		}
		return shortestMap;
	}

  	
    /**
     * wkt文件整合成FeatureCollection
     * @param WKTS
     * @param filedsList
     * @return
     * @throws Exception
     */
    public static FeatureCollection convertCollection(List<String> WKTS,List<ArrayList<String>> filedsList) throws Exception {
    	final SimpleFeatureType TYPE = DataUtilities.createType("Location","geom:Polygon:4490,"
    			+"pointNumb:String,"
    			+"plotArea:String,"
    			+"serial:String,"
    			+"plotName:String,"
    			+"geoType:String,"
    			+"frameNumb:String,"
    			+"use:String,"
    			+"remarks:String");
    	SimpleFeatureBuilder featureBuilder = new SimpleFeatureBuilder(TYPE);
    	GeometryFactory geometryFactory = new GeometryFactory();
    	WKTReader reader = new WKTReader( geometryFactory );
    	FeatureJSON fjson = new FeatureJSON(new GeometryJSON(15));
    	List<SimpleFeature> features = new ArrayList();
    	FeatureCollection<SimpleFeatureType, SimpleFeature> collection = new ListFeatureCollection(TYPE, features);
    	
    	for (int x =0;x< WKTS.size();x++){
			Polygon polygon = (Polygon)reader.read(WKTS.get(x));
		    featureBuilder.add(polygon);
		    SimpleFeature feature = featureBuilder.buildFeature(null);
		    for(int i = 0; i < feature.getAttributeCount();i++) {
		    	if(i == 0 ) {
		    		feature.setAttribute(i, polygon); //设置几何属性
		    	}else {
		    		if(filedsList.get(x).size() >= i-1) {
		    			feature.setAttribute(i, filedsList.get(x).get(i-1)); //设置其他属性
		    		}else {
		    			feature.setAttribute(i, " "); //设置其他属性
		    		}
		    	}
	    	}
		    features.add(feature);
    	}
    	FeatureIterator<SimpleFeature> envelopeIt = collection.features();
    	while(envelopeIt.hasNext()) {
    		SimpleFeature feature = envelopeIt.next();
    		Geometry geo = (Geometry)feature.getDefaultGeometry();
    		Iterator<Property> proIt =  feature.getProperties().iterator();
	  		while(proIt.hasNext()) {
	  			  Property property = proIt.next();
	  			  String name = property.getName().toString();
	  			  if(!name.equals("geom")) {
	  				  String value = property.getValue().toString();
	 	  			 System.out.print(name+" : "+value + " ");
	  			  }
	  			
	  		 }
	  		System.out.println();
    	}
    	return(collection);
	}
    
    /**
     * txt生成shp（供测试用）
     * @author longjie
     * @param txtPath
     * @param shpPath
     */
    public static void txtToShp(String txtPath,String shpPath) {
    	File f = new File(txtPath);
        FileInputStream fis = null;
        InputStreamReader isr = null;
        BufferedReader br = null;
        List<Geometry> geoList = new ArrayList<Geometry>();
        List<ArrayList<String>> filedsList = new ArrayList<ArrayList<String>>();
        CoordinateReferenceSystem sourceCRS = null;
        try {
            fis = new FileInputStream(f);
            isr = new InputStreamReader(fis);
            br = new BufferedReader(isr);
            String temp = null;
            ArrayList<String> buffer = new ArrayList<String>();
            boolean startFlag = false;
            
            ArrayList<String> zeroBuffer = new ArrayList<String>();
            String symbol = "";
            Map<Integer,ArrayList<String>> bufferMap = new HashMap<Integer,ArrayList<String>>(); 
            
            //逐行读取数据
            while ((temp = br.readLine()) != null) {
                if (temp.contains("@")) {
                    if (buffer.size() > 0) {
                  	  if(buffer.size() > 3) 
                  	  {
                  		  ArrayList<String> filed = new ArrayList<String>();
                            Geometry geo = convert(buffer,filed);
                            if(null == sourceCRS) {
                            	sourceCRS = CRSTransform.returnCRS(geo);
            				}
                            if(zeroBuffer.size() > 3) {
                          	  Geometry zeroGeo = interactorConvert(zeroBuffer);
                          	  geo = geo.union(zeroGeo);
                            }
                            ArrayList<Geometry> tempInteratorList = new ArrayList<Geometry>();
                            if(bufferMap.size() > 0) {
                          	  for (Map.Entry<Integer, ArrayList<String>> entry : bufferMap.entrySet()) {
                          		  Geometry tempGeo = interactorConvert(entry.getValue());
                          		  tempInteratorList.add(tempGeo);
                          	  }
                            }
                            
                            if(tempInteratorList.size() > 0) {
                          	  for(Geometry tempGeo : tempInteratorList) {
                          		  if(geo.intersects(tempGeo)) {
                          			  geo = geo.difference(tempGeo);
                          		  }
                          	  }
                            }
                            if(!geo.isEmpty()) {
                          	  filedsList.add(filed);
                                geoList.add(geo);
                            }
                            zeroBuffer.clear();
                            bufferMap.clear();
                            buffer.clear();
                  	  }
                    	 
                    } else {
                        startFlag = true;
                    }
                }
                if (startFlag) {
              	  
              	  if(buffer.size() == 0) {
              		  buffer.add(temp);
              		  zeroBuffer.add(temp);
              		  symbol = temp;
              	  }
              	  
              	  if(buffer.size() > 0) {
              		  if(temp.indexOf(",1,") > 0) {
              			  buffer.add(temp);
              		  }else if(temp.indexOf(",0,") > 0) {
              			  zeroBuffer.add(temp);
              		  }else {
              			  if(temp.indexOf("@") < 0) {
              				  String[] s = temp.split(",");
              				  String str = s[1];
              				  int symbolNum = Integer.parseInt(str);
              				  if(!bufferMap.containsKey(symbolNum)) {
              					  ArrayList<String> tempList = new ArrayList<String>();
              					  tempList.add(symbol);
              					  tempList.add(temp);
              					  bufferMap.put(symbolNum, tempList);
              				  }else {
              					  bufferMap.get(symbolNum).add(temp);
              				  }
              			  }
              		  }
              	  }
                }
            }
            
            ArrayList<String> filedList = new ArrayList<String>();
            Geometry lastGeo = convert(buffer,filedList);
            if(null == sourceCRS) {
            	sourceCRS = CRSTransform.returnCRS(lastGeo);
			}
            if(zeroBuffer.size() > 3) {
          	  Geometry zeroGeo = interactorConvert(zeroBuffer);
          	  lastGeo = lastGeo.union(zeroGeo);
            }
            ArrayList<Geometry> tempInteratorList = new ArrayList<Geometry>();
            if(bufferMap.size() > 0) {
          	  for (Map.Entry<Integer, ArrayList<String>> entry : bufferMap.entrySet()) {
          		  Geometry tempGeo = interactorConvert(entry.getValue());
          		  tempInteratorList.add(tempGeo);
          	  }
            }
            
            if(tempInteratorList.size() > 0 && !lastGeo.isEmpty()) {
          	  for(Geometry tempGeo : tempInteratorList) {
          		  lastGeo = lastGeo.difference(tempGeo);
          	  }
            }
            
            if(!lastGeo.isEmpty()) {
          	  filedsList.add(filedList);
                geoList.add(lastGeo);
            }
          
            //用wkt生成LineString
            WKTReader reader = new WKTReader( gf );
            List<String> wktList = new ArrayList<String>(); 
            for(int i = 0; i < geoList.size(); i++) {
            	Polygon polygon = (Polygon)reader.read(geoList.get(i).toString());
            	wktList.add(polygon.toString());
            }

            //创建shape文件对象
            File file = new File(shpPath);  
            Map<String, Serializable> params = new HashMap<String, Serializable>();  
            params.put( ShapefileDataStoreFactory.URLP.key, file.toURI().toURL() );  
            ShapefileDataStore ds  = (ShapefileDataStore) new ShapefileDataStoreFactory().createNewDataStore(params);  
            
            //设置编码  
            Charset charset = Charset.forName("GBK");  
            ds.setCharset(charset);
            
            final SimpleFeatureType TYPE = DataUtilities.createType("Location","geom,"
        			+"pointNumb:String,"
        			+"plotArea:String,"
        			+"serial:String,"
        			+"plotName:String,"
        			+"geoType:String,"
        			+"frameNumb:String,"
        			+"use:String,"
        			+"remarks:String");
        	SimpleFeatureBuilder featureBuilder = new SimpleFeatureBuilder(TYPE);
        	
        	 SimpleFeatureTypeBuilder tb = new SimpleFeatureTypeBuilder();  
             tb.setCRS(sourceCRS);  
             tb.setName("shapefile");  
             tb.add("the_geom", MultiPolygon.class);
             //设置属性
	  		 String pointNumb = "pointNumb";
	  		 tb.add(pointNumb, String.class);  
	  		 
	  		 String name = "pointNumb";
	  		 tb.add(name, String.class); 
	  		 
	  		 String plotArea = "plotArea";
	  		 tb.add(plotArea, String.class); 
	  		 
	  		 String serial = "serial";
	  		 tb.add(serial, String.class); 
	  		 
	  		 String plotName = "plotName";
	  		 tb.add(plotName, String.class); 
	  		 
	  		 String frameNumb = "frameNumb";
	  		 tb.add(frameNumb, String.class); 
	  		 
	  		 String use = "use";
	  		 tb.add(use, String.class); 
	  		 
	  		 String remarks = "remarks";
	  		 tb.add(remarks, String.class); 

             ds.createSchema(tb.buildFeatureType());
        	FeatureWriter<SimpleFeatureType, SimpleFeature> writer = ds.getFeatureWriter(ds.getTypeNames()[0], Transaction.AUTO_COMMIT);  
        	for (int x =0;x< wktList.size();x++){
    			Polygon polygon = (Polygon)reader.read(wktList.get(x));
    		    SimpleFeature feature = writer.next();
    		    for(int i = 0; i < feature.getAttributeCount();i++) {
    		    	if(i == 0 ) {
    		    		feature.setAttribute(i, polygon); //设置几何属性
    		    	}else {
    		    		if(filedsList.get(x).size() >= i-1) {
    		    			feature.setAttribute(i, filedsList.get(x).get(i-1)); //设置其他属性
    		    		}else {
    		    			feature.setAttribute(i, " "); //设置其他属性
    		    		}
    		    	}
    	    	}
    		    writer.write(); 
        	}
        	 writer.close();
        	 ds.dispose();
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if (br != null) {
                try {
                    br.close();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
            if (isr != null) {
                try {
                    isr.close();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
            if (fis != null) {
                try {
                    fis.close();
                } catch (IOException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }
    }
  	
}
