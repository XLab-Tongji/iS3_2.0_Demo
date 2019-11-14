# -*- coding:gb2312 -*-
import is3

from System.Collections.ObjectModel import ObservableCollection
from System.Windows.Media import Colors


def addBaseMap():
    is3.mainframe.LoadProject('TONGJI.xml')
    is3.prj = is3.mainframe.prj
    is3.MainframeWrapper.loadDomainPanels()
    
    emap = is3.EngineeringMap('BaseMap',
                              13525317, 3669179, 13526694, 3669709, 0.1)
    emap.LocalTileFileName1 = 'tongji.tpk'
    emap.LocalTileFileName2 = 'tongjibase.tpk'
    emap.LocalGeoDbFileName = 'tongji.geodatabase'
    emap.MapType = is3.EngineeringMapType.FootPrintMap;

    viewWP = is3.MainframeWrapper.addView(emap)
    return viewWP


def addBhLayer(viewWP):    
    layerDef = is3.LayerDef()
    layerDef.Name = 'Borehole'
    layerDef.GeometryType = is3.GeometryType.Point
    layerDef.Color = Colors.Blue
    layerDef.MarkerSize = 12
    layerDef.MarkerStyle = is3.SimpleMarkerStyle.Circle
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    bhLayerWrapper = is3.addGdbLayer(viewWP, layerDef)

    layerDef = is3.LayerDef()
    layerDef.Name = 'Mon'
    layerDef.GeometryType = is3.GeometryType.Point
    layerDef.Color = Colors.Yellow
    layerDef.MarkerSize = 12
    layerDef.MarkerStyle = is3.SimpleMarkerStyle.Circle
    layerDef.EnableLabel = True
    layerDef.LabelTextExpression = '[Name]'
    bhLayerWrapper = is3.addGdbLayer(viewWP, layerDef)
    return bhLayerWrapper


def add3dview():
    print ("--- Add 3D map ---")
    is3.addView3d('Map3D', 'tongji.unity3d')    #--->注：加载三维发布的.unity3d文件，替换成对应的工程ID
    return

def Load():
    global viewWP1, viewWP2, viewWP3, safe_view

    print("--- Add base map ---")
    viewWP1 = addBaseMap()
    bhLayerWP = addBhLayer(viewWP1)

    print ("--- Add a empty longitudinal profile map ---")
    emap = is3.EngineeringMap('profile1', 0, 0, 100, 100, 0.01)
    emap.MapType = is3.EngineeringMapType.GeneralProfileMap;
    safe_view = is3.MainframeWrapper.addView(emap)
    tilefile = is3.Runtime.tilePath + "\\Empty.tpk"
    safe_view.addLocalTiledLayer(tilefile, 'baselayer')
    add3dview()

Load()
