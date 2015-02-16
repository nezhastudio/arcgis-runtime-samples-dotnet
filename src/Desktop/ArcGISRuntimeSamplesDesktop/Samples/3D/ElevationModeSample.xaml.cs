﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ArcGISRuntime.Samples.Desktop
{
	/// <summary>
	/// Demonstrates elevevation mode works with the graphics.
	/// </summary>
	/// <title>3D Elevation mode</title>
	/// <category>3D</category>
	/// <subcategory>Mapping</subcategory>
	public partial class ElevationModeSample : UserControl
	{
		private Esri.ArcGISRuntime.Geometry.PointCollection _coordinates;
		private GraphicsLayer _absoluteGraphicsLayer;
		private GraphicsLayer _drapedGraphicsLayer;
		private GraphicsLayer _relativeGraphicsLayer;

		public ElevationModeSample()
		{
			InitializeComponent();
			CreatePoints();

			_absoluteGraphicsLayer = MySceneView.Scene.Layers["AbsoluteModeGraphicsLayer"] as GraphicsLayer;
			_drapedGraphicsLayer = MySceneView.Scene.Layers["DrapedModeGraphicsLayer"] as GraphicsLayer;
			_relativeGraphicsLayer= MySceneView.Scene.Layers["RelativeModeGraphicsLayer"] as GraphicsLayer;

			MySceneView.SpatialReferenceChanged += MySceneView_SpatialReferenceChanged;
		}

		private void CreatePoints()
		{
			_coordinates = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
			
			_coordinates.Add(new MapPoint(-106.981, 39.028, 6000, SpatialReferences.Wgs84));
			_coordinates.Add(new MapPoint(-106.956, 39.081, 6000, SpatialReferences.Wgs84));
			_coordinates.Add(new MapPoint(-106.869, 39.081, 6000, SpatialReferences.Wgs84));
			_coordinates.Add(new MapPoint(-106.879, 39.014, 6000, SpatialReferences.Wgs84));
			
		}

		private void MySceneView_SpatialReferenceChanged(object sender, System.EventArgs e)
		{
			MySceneView.SpatialReferenceChanged -= MySceneView_SpatialReferenceChanged;

			try
			{
				// Set initial viewpoint
				MySceneView.SetView(
					new Viewpoint3D(
						new MapPoint(-106.882128302391, 38.7658957449754, 12994.1727461051),
						358.607816178049,
						70.0562968167998));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error occured while setting initial viewpoint",
					"An error occured");
				Debug.WriteLine(ex.ToString());
			}
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e)
		{
			// Clear all existing graphics
			var graphicLayers = MySceneView.Scene.Layers.OfType<GraphicsLayer>();
			foreach (GraphicsLayer gl in graphicLayers)
				gl.Graphics.Clear();

			string shapeTag = (string)((RadioButton)sender).Tag;
			switch (shapeTag)
			{
				case "Point":
					// Add coordinates to all graphics layers using GraphicCollection and populate the data
					GraphicCollection absoluteCollection = new GraphicCollection();
					GraphicCollection relativeCollection = new GraphicCollection();
					GraphicCollection drapedCollection = new GraphicCollection();
					
					foreach (MapPoint point in _coordinates)
					{
						// Use graphics with different ElevationModes
						absoluteCollection.Add(new Graphic(point, GetPointSymbol(ElevationMode.Absolute)));
						relativeCollection.Add(new Graphic(point, GetPointSymbol(ElevationMode.Relative)));
						drapedCollection.Add(new Graphic(point, GetPointSymbol(ElevationMode.Draped)));
					}

					_absoluteGraphicsLayer.Graphics.AddRange(absoluteCollection);
					_drapedGraphicsLayer.Graphics.AddRange(drapedCollection);
					_relativeGraphicsLayer.Graphics.AddRange(relativeCollection);
					break;
				case "Line":
					// Create polylines with different ElevationModes
					var line = new Polyline(_coordinates, SpatialReferences.Wgs84);
					_absoluteGraphicsLayer.Graphics.Add(
						new Graphic(
							line, 
							GetPolylineSymbol(ElevationMode.Absolute)));
					_drapedGraphicsLayer.Graphics.Add(
						new Graphic(
							line, 
							GetPolylineSymbol(ElevationMode.Draped)));
					_relativeGraphicsLayer.Graphics.Add(
						new Graphic(
							line, 
							GetPolylineSymbol(ElevationMode.Relative)));
					break;
				case "Polygon":
					// Create polygons using different ElevationModes
					var polygon = new Polygon(_coordinates, SpatialReferences.Wgs84);
					_absoluteGraphicsLayer.Graphics.Add(
						new Graphic(polygon, GetPolygonSymbol(ElevationMode.Absolute)));
					_drapedGraphicsLayer.Graphics.Add(
						new Graphic(polygon, GetPolygonSymbol(ElevationMode.Draped)));
					_relativeGraphicsLayer.Graphics.Add(
						new Graphic(polygon, GetPolygonSymbol(ElevationMode.Relative)));
					break;
			}
		}

		private Symbol GetPolylineSymbol(ElevationMode mode)
		{
			SimpleLineSymbol sls = new SimpleLineSymbol();
			sls.Style = SimpleLineStyle.Solid;
			sls.Color = mode == ElevationMode.Absolute ? Colors.Red : mode == ElevationMode.Draped ? Colors.Yellow : Colors.LightBlue;
			sls.Width = 4;
			return sls;
		}

		private Symbol GetPolygonSymbol(ElevationMode mode)
		{
			SimpleFillSymbol sfs = new SimpleFillSymbol();
			sfs.Style = SimpleFillStyle.Solid;
			sfs.Color = mode == ElevationMode.Absolute ? Colors.Red : mode == ElevationMode.Draped ? Colors.Yellow : Colors.LightBlue;
			sfs.Outline = new SimpleLineSymbol() { Color = Colors.Red, Width = 2 };
			return sfs;
		}

		private Symbol GetPointSymbol(ElevationMode mode)
		{
			SimpleMarkerSymbol sms = new SimpleMarkerSymbol();
			sms.Style = SimpleMarkerStyle.Circle;
			sms.Color = mode == ElevationMode.Absolute ? Colors.Red : mode == ElevationMode.Draped ? Colors.Yellow : Colors.LightBlue;
			sms.Size = 20;
			return sms;
		}
	}
}
