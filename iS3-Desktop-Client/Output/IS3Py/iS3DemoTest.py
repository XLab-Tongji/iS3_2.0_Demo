# -*- coding:gb2312 -*-
import iS3
iS3.mainframe.LoadProject('iS3DemoTest.xml')
iS3.prj = iS3.mainframe.prj
iS3.MainframeWrapper.loadDomainPanels()
for emap in iS3.prj.projDef.EngineeringMaps:
    iS3.MainframeWrapper.addView(emap)
iS3.addView3d('Map3D', 'iS3DemoTest.unity3d')
