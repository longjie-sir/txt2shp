package utils;

import java.io.File;
import java.nio.charset.Charset;
import java.text.NumberFormat;

import org.geotools.data.FeatureSource;
import org.geotools.data.shapefile.ShapefileDataStore;
import org.geotools.feature.FeatureCollection;
import org.geotools.feature.FeatureIterator;
import org.geotools.geometry.jts.JTS;
import org.geotools.referencing.CRS;
import org.opengis.feature.simple.SimpleFeature;
import org.opengis.feature.simple.SimpleFeatureType;
import org.opengis.geometry.MismatchedDimensionException;
import org.opengis.referencing.FactoryException;
import org.opengis.referencing.crs.CoordinateReferenceSystem;
import org.opengis.referencing.operation.MathTransform;
import org.opengis.referencing.operation.TransformException;

import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Envelope;
import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.Point;
import com.vividsolutions.jts.geom.Polygon;

public class CRSTransform {

	final static String cgcs_z33 = "PROJCS[\"CGCS2000_3_degree_Gauss_Kruger_zone_33\",GEOGCS[\"GCS_China Geodetic Coordinate System 2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",99],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",33500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
	final static String cgcs_z34 = "PROJCS[\"CGCS2000_3_degree_Gauss_Kruger_zone_34\",GEOGCS[\"GCS_China Geodetic Coordinate System 2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",102],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",34500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
	final static String cgcs2000 = "GEOGCS[\"China Geodetic Coordinate System 2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
	final static String cgcs_z35 = "PROJCS[\"CGCS2000_3_degree_Gauss_Kruger_zone_35\",GEOGCS[\"GCS_China Geodetic Coordinate System 2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",105],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",35500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
	final static String cgcs_z36 = "PROJCS[\"CGCS2000_3_degree_Gauss_Kruger_zone_36\",GEOGCS[\"GCS_China Geodetic Coordinate System 2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",108],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",36500000],PARAMETER[\"false_northing\",0],UNIT[\"Meter\",1]]";
	
	private static CoordinateReferenceSystem crsCGCS2000 = null;
	private static CoordinateReferenceSystem crsCGCS2000Z33 = null;
	private static CoordinateReferenceSystem crsCGCS2000Z34 = null;
	private static CoordinateReferenceSystem crsCGCS2000Z35 = null;
	private static CoordinateReferenceSystem crsCGCS2000Z36 = null;

	private static GeometryFactory gf = new GeometryFactory();

	public static void init() {
		try {
			if (crsCGCS2000 == null) {
				crsCGCS2000 = CRS.parseWKT(cgcs2000);
			}
			if (crsCGCS2000Z33 == null) {
				crsCGCS2000Z33 = CRS.parseWKT(cgcs_z33);
			}
			if (crsCGCS2000Z34 == null) {
				crsCGCS2000Z34 = CRS.parseWKT(cgcs_z34);
			}
			if (crsCGCS2000Z35 == null) {
				crsCGCS2000Z35 = CRS.parseWKT(cgcs_z35);
			}
			if (crsCGCS2000Z36 == null) {
				crsCGCS2000Z36 = CRS.parseWKT(cgcs_z36);
			}
		} catch (FactoryException e) {
			e.printStackTrace();
		}
	}

	public static String getGeoZone3D(Geometry geo) {
		String result = null;
		Envelope ei = geo.getEnvelopeInternal();
		double minY = ei.getMinY() - 1;
		double maxY = ei.getMaxY() + 1;
		double minX = ei.getMinX();
		double maxX = ei.getMaxX();
		if (maxX > 300) {
			NumberFormat nf = NumberFormat.getInstance();
			nf.setGroupingUsed(false);
			String minxStr = nf.format(minX);
			String maxxStr = nf.format(maxX);
			minxStr = minxStr.substring(0, 2);
			maxxStr = maxxStr.substring(0, 2);
			if ("33".equals(minxStr) && "33".equals(maxxStr)) {
				result = "z33";
			} else if ("34".equals(minxStr) && "34".equals(maxxStr)) {
				result = "z34";
			} else if ("35".equals(minxStr) && "35".equals(maxxStr)) {
				result = "z35";
			} else if ("36".equals(minxStr) && "36".equals(maxxStr)) {
				result = "z36";
			}
		} else {
			if (minX >= 97.5 && maxX <= 100.5) {
				result = "z33";
			} else if (minX >= 100.5 && maxX <= 103.5) {
				result = "z34";
			} else if (minX >= 103.5 && maxX <= 106.5) {
				result = "z35";
			} else if (minX >= 106.5 && maxX <= 108.5) {
				result = "z36";
			} else {
				double maxArea = Double.MIN_VALUE;
				Coordinate[] coordinates = new Coordinate[] { new Coordinate(97.5, minY), new Coordinate(97.5, maxY),
						new Coordinate(100.5, maxY), new Coordinate(100.5, minY), new Coordinate(97.5, minY) };
				Polygon z33 = gf.createPolygon(gf.createLinearRing(coordinates), null);
				Geometry intersection1 = null;
				try {
					intersection1 = geo.intersection(z33);
				} catch (Exception e) {
					intersection1 = geo.buffer(0).intersection(z33.buffer(0));
				}
				double area1 = intersection1.getArea();
				if (area1 > maxArea) {
					result = "z33";
					maxArea = area1;
				}

				coordinates = new Coordinate[] { new Coordinate(100.5, minY), new Coordinate(100.5, maxY),
						new Coordinate(103.5, maxY), new Coordinate(103.5, minY), new Coordinate(100.5, minY) };
				Polygon z34 = gf.createPolygon(gf.createLinearRing(coordinates), null);
				Geometry intersection2 = null;
				try {
					intersection2 = geo.intersection(z34);
				} catch (Exception e) {
					intersection2 = geo.buffer(0).intersection(z34.buffer(0));
				}
				double area2 = intersection2.getArea();
				if (area2 > maxArea) {
					result = "z34";
					maxArea = area2;
				}

				coordinates = new Coordinate[] { new Coordinate(103.5, minY), new Coordinate(103.5, maxY),
						new Coordinate(106.5, maxY), new Coordinate(106.5, minY), new Coordinate(103.5, minY) };
				Polygon z35 = gf.createPolygon(gf.createLinearRing(coordinates), null);
				Geometry intersection3 = null;
				try {
					intersection3 = geo.intersection(z35);
				} catch (Exception e) {
					intersection3 = geo.buffer(0).intersection(z35.buffer(0));
				}
				double area3 = intersection3.getArea();
				if (area3 > maxArea) {
					result = "z35";
				}
				
				
				coordinates = new Coordinate[] { new Coordinate(106.5, minY), new Coordinate(106.5, maxY),
						new Coordinate(108.5, maxY), new Coordinate(108.5, minY), new Coordinate(106.5, minY) };
				Polygon z36 = gf.createPolygon(gf.createLinearRing(coordinates), null);
				Geometry intersection4 = null;
				try {
					intersection4 = geo.intersection(z36);
				} catch (Exception e) {
					intersection4 = geo.buffer(0).intersection(z36.buffer(0));
				}
				double area4 = intersection4.getArea();
				if (area4 > maxArea) {
					result = "z36";
				}
			}
		}
		return result;
	}

	public static String getGeoZone6D(Geometry geo) {
		String result = null;
		Envelope ei = geo.getEnvelopeInternal();
		double minY = ei.getMinY() - 1;
		double maxY = ei.getMaxY() + 1;
		double minX = ei.getMinX();
		double maxX = ei.getMaxX();
		if (minX >= 93 && maxX <= 99) {
			result = "E99";
		} else if (minX >= 99 && maxX <= 105) {
			result = "E105";
		} else if (minX >= 105 && maxX <= 108) {
			result = "E108";
		} else {
			double maxArea = Double.MIN_VALUE;
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(93, minY), new Coordinate(93, maxY),
					new Coordinate(99, maxY), new Coordinate(99, minY), new Coordinate(93, minY) };
			Polygon E99 = gf.createPolygon(gf.createLinearRing(coordinates), null);
			Geometry intersection1 = null;
			try {
				intersection1 = geo.intersection(E99);
			} catch (Exception e) {
				intersection1 = geo.buffer(0).intersection(E99.buffer(0));
			}
			double area1 = intersection1.getArea();
			if (area1 > maxArea) {
				result = "E99";
				maxArea = area1;
			}

			coordinates = new Coordinate[] { new Coordinate(99, minY), new Coordinate(99, maxY),
					new Coordinate(105, maxY), new Coordinate(105, minY), new Coordinate(99, minY) };
			Polygon E105 = gf.createPolygon(gf.createLinearRing(coordinates), null);
			Geometry intersection2 = null;
			try {
				intersection2 = geo.intersection(E105);
			} catch (Exception e) {
				intersection2 = geo.buffer(0).intersection(E105.buffer(0));
			}
			double area2 = intersection2.getArea();
			if (area2 > maxArea) {
				result = "E105";
				maxArea = area2;
			}
			
			coordinates = new Coordinate[] { new Coordinate(105, minY), new Coordinate(105, maxY),
					new Coordinate(108, maxY), new Coordinate(108, minY), new Coordinate(105, minY) };
			Polygon E108 = gf.createPolygon(gf.createLinearRing(coordinates), null);
			Geometry intersection3 = null;
			try {
				intersection3 = geo.intersection(E108);
			} catch (Exception e) {
				intersection3 = geo.buffer(0).intersection(E108.buffer(0));
			}
			double area3 = intersection3.getArea();
			if (area3 > maxArea) {
				result = "E108";
				maxArea = area3;
			}
			
		}
		return result;
	}

	public static Geometry trans2KGeoToProject(Geometry geo, String zone) {
		Geometry result = null;
		init();
		CoordinateReferenceSystem target = null;
		if (zone.equals("z33")) {
			target = crsCGCS2000Z33;
		} else if (zone.equals("z34")) {
			target = crsCGCS2000Z34;
		} else if (zone.equals("z35")) {
			target = crsCGCS2000Z35;
		} else if (zone.equals("z36")) {
			target = crsCGCS2000Z36;
		}
		MathTransform transform = null;
		try {
			transform = CRS.findMathTransform(crsCGCS2000, target);
			result = JTS.transform(geo, transform);
		} catch (FactoryException e) {
			e.printStackTrace();
		} catch (MismatchedDimensionException e) {
			e.printStackTrace();
		} catch (TransformException e) {
			e.printStackTrace();
		}

		return result;
	}

	public static Geometry trans2KProjectToGeo(Geometry geo, String zone) {
		Geometry result = null;
		init();
		CoordinateReferenceSystem source = null;
		if (zone.equals("z33")) {
			source = crsCGCS2000Z33;
		} else if (zone.equals("z34")) {
			source = crsCGCS2000Z34;
		} else if (zone.equals("z35")) {
			source = crsCGCS2000Z35;
		} else if (zone.equals("z36")) {
			source = crsCGCS2000Z36;
		}
		MathTransform transform = null;
		try {
			transform = CRS.findMathTransform(source, crsCGCS2000);
			result = JTS.transform(geo, transform);
		} catch (FactoryException e) {
			e.printStackTrace();
		} catch (MismatchedDimensionException e) {
			e.printStackTrace();
		} catch (TransformException e) {
			e.printStackTrace();
		}

		return result;
	}

	public static Geometry transCGCS2000CRSProjectToGeo(Geometry geom) {
		Geometry result = null;
		init();
		Point interiorPoint = geom.getInteriorPoint();
		double x = interiorPoint.getX();
		CoordinateReferenceSystem source = null;
		if (x >= 3.334620390742564E7 && x <= 3.365379609257436E7) {
			source = crsCGCS2000Z33;
		} else if (x > 3.434620390742564E7 && x <= 3.465379609257436E7) {
			source = crsCGCS2000Z34;
		} else if (x > 3.5346203907425635E7 && x <= 3.565379609257436E7) {
			source = crsCGCS2000Z35;
		} else if (x > 3.6346203907425635E7 && x <= 3.665379609257436E7) {
			source = crsCGCS2000Z36;
		}
		try {
			MathTransform transform = CRS.findMathTransform(source, crsCGCS2000);
			result = JTS.transform(geom, transform);
		} catch (FactoryException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (MismatchedDimensionException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (TransformException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return result;
	}

	public static Geometry transPartCGCS2000CRSGeoToProject(Geometry geo, CoordinateReferenceSystem target,
			CoordinateReferenceSystem source) {
		Geometry result = null;
		init();
		try {
			MathTransform transform = CRS.findMathTransform(source, target);
			result = JTS.transform(geo, transform);
		} catch (FactoryException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (MismatchedDimensionException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (TransformException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return result;
	}
	
	public static CoordinateReferenceSystem returnCRS(Geometry geom) {
		init();
		Point interiorPoint = geom.getInteriorPoint();
		double x = interiorPoint.getX();
		CoordinateReferenceSystem source = null;
		if (x >= 3.334620390742564E7 && x <= 3.365379609257436E7) {
			source = crsCGCS2000Z33;
		} else if (x > 3.434620390742564E7 && x <= 3.465379609257436E7) {
			source = crsCGCS2000Z34;
		} else if (x > 3.5346203907425635E7 && x <= 3.565379609257436E7) {
			source = crsCGCS2000Z35;
		} else if (x > 3.6346203907425635E7 && x <= 3.665379609257436E7) {
			source = crsCGCS2000Z36;
		}
		return source;
	}

	public static String returnZone(Geometry geom) {
		Point interiorPoint = geom.getInteriorPoint();
		double x = interiorPoint.getX();
		if (x >= 3.334620390742564E7 && x <= 3.365379609257436E7) {
			return "z33";
		} else if (x > 3.434620390742564E7 && x <= 3.465379609257436E7) {
			return "z34";
		} else if (x > 3.5346203907425635E7 && x <= 3.565379609257436E7) {
			return "z35";
		} else if (x > 3.6346203907425635E7 && x <= 3.665379609257436E7) {
			return "z36";
		}else {
			return "z36";
		}
	}
	
	public static String returnShpZone(String shpPath) throws Exception {
		ShapefileDataStore shpDataStore = new ShapefileDataStore(new File(shpPath).toURI().toURL());
		shpDataStore.setCharset(Charset.forName("GBK"));  
		String zone = null;
		try {
			FeatureSource<SimpleFeatureType, SimpleFeature> featureSource = (FeatureSource<SimpleFeatureType, SimpleFeature>)shpDataStore.getFeatureSource(shpDataStore.getTypeNames()[0]);  
			FeatureCollection<SimpleFeatureType, SimpleFeature> featureResult = featureSource.getFeatures();  	
			FeatureIterator<SimpleFeature> It = featureResult.features();
			if(null == zone && It.hasNext()) {
				SimpleFeature lxdwFeature = It.next();
				Geometry geometry = (Geometry) lxdwFeature.getDefaultGeometry();
				zone = returnZone(geometry);
			}
			It.close();
		}catch(Exception ex) {
			ex.printStackTrace();
		}finally {
			if(null != shpDataStore) {
				shpDataStore.dispose();
			}
		}
		if(null == zone) {
			zone = "z36";
		}
		return zone;
	}
	
	public static String returnCollectionZone(FeatureCollection<SimpleFeatureType, SimpleFeature> collection) {
		String zone = null;	
		FeatureIterator<SimpleFeature> It = collection.features();
		if(null == zone && It.hasNext()) {
			SimpleFeature lxdwFeature = It.next();
			Geometry geometry = (Geometry) lxdwFeature.getDefaultGeometry();
			zone = returnZone(geometry);
		}
		It.close();
		if(null == zone) {
			zone = "z36";
		}
		return zone;
	}
	
	public static void main(String[] args) {
		//GeometryFactory gf=new GeometryFactory();
		//Point point1 = gf.createPoint(new Coordinate(36361928.0271, 2904840.471000001));
//		Point point2 = gf.createPoint(new Coordinate(3.35342973005E7,2720165.1521));
		// 
		//System.out.println(ppoint.toText());
//		System.out.println(point2.distance(ppoint));
//		GeoObject go=new GeoObject(new JSONObject(), point1);
//		ArrayList<GeoObject> geos=new ArrayList<>();
//		geos.add(go);
//		GeoObjectToShp.geoJsonToShp(geos, "D:/projectTest.shp", "Point");
	}
}
