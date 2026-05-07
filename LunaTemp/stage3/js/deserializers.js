var Deserializers = {}
Deserializers["UnityEngine.JointSpring"] = function (request, data, root) {
  var i482 = root || request.c( 'UnityEngine.JointSpring' )
  var i483 = data
  i482.spring = i483[0]
  i482.damper = i483[1]
  i482.targetPosition = i483[2]
  return i482
}

Deserializers["UnityEngine.JointMotor"] = function (request, data, root) {
  var i484 = root || request.c( 'UnityEngine.JointMotor' )
  var i485 = data
  i484.m_TargetVelocity = i485[0]
  i484.m_Force = i485[1]
  i484.m_FreeSpin = i485[2]
  return i484
}

Deserializers["UnityEngine.JointLimits"] = function (request, data, root) {
  var i486 = root || request.c( 'UnityEngine.JointLimits' )
  var i487 = data
  i486.m_Min = i487[0]
  i486.m_Max = i487[1]
  i486.m_Bounciness = i487[2]
  i486.m_BounceMinVelocity = i487[3]
  i486.m_ContactDistance = i487[4]
  i486.minBounce = i487[5]
  i486.maxBounce = i487[6]
  return i486
}

Deserializers["UnityEngine.JointDrive"] = function (request, data, root) {
  var i488 = root || request.c( 'UnityEngine.JointDrive' )
  var i489 = data
  i488.m_PositionSpring = i489[0]
  i488.m_PositionDamper = i489[1]
  i488.m_MaximumForce = i489[2]
  i488.m_UseAcceleration = i489[3]
  return i488
}

Deserializers["UnityEngine.SoftJointLimitSpring"] = function (request, data, root) {
  var i490 = root || request.c( 'UnityEngine.SoftJointLimitSpring' )
  var i491 = data
  i490.m_Spring = i491[0]
  i490.m_Damper = i491[1]
  return i490
}

Deserializers["UnityEngine.SoftJointLimit"] = function (request, data, root) {
  var i492 = root || request.c( 'UnityEngine.SoftJointLimit' )
  var i493 = data
  i492.m_Limit = i493[0]
  i492.m_Bounciness = i493[1]
  i492.m_ContactDistance = i493[2]
  return i492
}

Deserializers["UnityEngine.WheelFrictionCurve"] = function (request, data, root) {
  var i494 = root || request.c( 'UnityEngine.WheelFrictionCurve' )
  var i495 = data
  i494.m_ExtremumSlip = i495[0]
  i494.m_ExtremumValue = i495[1]
  i494.m_AsymptoteSlip = i495[2]
  i494.m_AsymptoteValue = i495[3]
  i494.m_Stiffness = i495[4]
  return i494
}

Deserializers["UnityEngine.JointAngleLimits2D"] = function (request, data, root) {
  var i496 = root || request.c( 'UnityEngine.JointAngleLimits2D' )
  var i497 = data
  i496.m_LowerAngle = i497[0]
  i496.m_UpperAngle = i497[1]
  return i496
}

Deserializers["UnityEngine.JointMotor2D"] = function (request, data, root) {
  var i498 = root || request.c( 'UnityEngine.JointMotor2D' )
  var i499 = data
  i498.m_MotorSpeed = i499[0]
  i498.m_MaximumMotorTorque = i499[1]
  return i498
}

Deserializers["UnityEngine.JointSuspension2D"] = function (request, data, root) {
  var i500 = root || request.c( 'UnityEngine.JointSuspension2D' )
  var i501 = data
  i500.m_DampingRatio = i501[0]
  i500.m_Frequency = i501[1]
  i500.m_Angle = i501[2]
  return i500
}

Deserializers["UnityEngine.JointTranslationLimits2D"] = function (request, data, root) {
  var i502 = root || request.c( 'UnityEngine.JointTranslationLimits2D' )
  var i503 = data
  i502.m_LowerTranslation = i503[0]
  i502.m_UpperTranslation = i503[1]
  return i502
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh"] = function (request, data, root) {
  var i504 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh' )
  var i505 = data
  i504.name = i505[0]
  i504.halfPrecision = !!i505[1]
  i504.useSimplification = !!i505[2]
  i504.useUInt32IndexFormat = !!i505[3]
  i504.vertexCount = i505[4]
  i504.aabb = i505[5]
  var i507 = i505[6]
  var i506 = []
  for(var i = 0; i < i507.length; i += 1) {
    i506.push( !!i507[i + 0] );
  }
  i504.streams = i506
  i504.vertices = i505[7]
  var i509 = i505[8]
  var i508 = []
  for(var i = 0; i < i509.length; i += 1) {
    i508.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh', i509[i + 0]) );
  }
  i504.subMeshes = i508
  var i511 = i505[9]
  var i510 = []
  for(var i = 0; i < i511.length; i += 16) {
    i510.push( new pc.Mat4().setData(i511[i + 0], i511[i + 1], i511[i + 2], i511[i + 3],  i511[i + 4], i511[i + 5], i511[i + 6], i511[i + 7],  i511[i + 8], i511[i + 9], i511[i + 10], i511[i + 11],  i511[i + 12], i511[i + 13], i511[i + 14], i511[i + 15]) );
  }
  i504.bindposes = i510
  var i513 = i505[10]
  var i512 = []
  for(var i = 0; i < i513.length; i += 1) {
    i512.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape', i513[i + 0]) );
  }
  i504.blendShapes = i512
  return i504
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh"] = function (request, data, root) {
  var i518 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh' )
  var i519 = data
  i518.triangles = i519[0]
  return i518
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape"] = function (request, data, root) {
  var i524 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape' )
  var i525 = data
  i524.name = i525[0]
  var i527 = i525[1]
  var i526 = []
  for(var i = 0; i < i527.length; i += 1) {
    i526.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame', i527[i + 0]) );
  }
  i524.frames = i526
  return i524
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material"] = function (request, data, root) {
  var i528 = root || new pc.UnityMaterial()
  var i529 = data
  i528.name = i529[0]
  request.r(i529[1], i529[2], 0, i528, 'shader')
  i528.renderQueue = i529[3]
  i528.enableInstancing = !!i529[4]
  var i531 = i529[5]
  var i530 = []
  for(var i = 0; i < i531.length; i += 1) {
    i530.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter', i531[i + 0]) );
  }
  i528.floatParameters = i530
  var i533 = i529[6]
  var i532 = []
  for(var i = 0; i < i533.length; i += 1) {
    i532.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter', i533[i + 0]) );
  }
  i528.colorParameters = i532
  var i535 = i529[7]
  var i534 = []
  for(var i = 0; i < i535.length; i += 1) {
    i534.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter', i535[i + 0]) );
  }
  i528.vectorParameters = i534
  var i537 = i529[8]
  var i536 = []
  for(var i = 0; i < i537.length; i += 1) {
    i536.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter', i537[i + 0]) );
  }
  i528.textureParameters = i536
  var i539 = i529[9]
  var i538 = []
  for(var i = 0; i < i539.length; i += 1) {
    i538.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag', i539[i + 0]) );
  }
  i528.materialFlags = i538
  return i528
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter"] = function (request, data, root) {
  var i542 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter' )
  var i543 = data
  i542.name = i543[0]
  i542.value = i543[1]
  return i542
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter"] = function (request, data, root) {
  var i546 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter' )
  var i547 = data
  i546.name = i547[0]
  i546.value = new pc.Color(i547[1], i547[2], i547[3], i547[4])
  return i546
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter"] = function (request, data, root) {
  var i550 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter' )
  var i551 = data
  i550.name = i551[0]
  i550.value = new pc.Vec4( i551[1], i551[2], i551[3], i551[4] )
  return i550
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter"] = function (request, data, root) {
  var i554 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter' )
  var i555 = data
  i554.name = i555[0]
  request.r(i555[1], i555[2], 0, i554, 'value')
  return i554
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag"] = function (request, data, root) {
  var i558 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag' )
  var i559 = data
  i558.name = i559[0]
  i558.enabled = !!i559[1]
  return i558
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Texture2D"] = function (request, data, root) {
  var i560 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Texture2D' )
  var i561 = data
  i560.name = i561[0]
  i560.width = i561[1]
  i560.height = i561[2]
  i560.mipmapCount = i561[3]
  i560.anisoLevel = i561[4]
  i560.filterMode = i561[5]
  i560.hdr = !!i561[6]
  i560.format = i561[7]
  i560.wrapMode = i561[8]
  i560.alphaIsTransparency = !!i561[9]
  i560.alphaSource = i561[10]
  i560.graphicsFormat = i561[11]
  i560.sRGBTexture = !!i561[12]
  i560.desiredColorSpace = i561[13]
  i560.wrapU = i561[14]
  i560.wrapV = i561[15]
  return i560
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Transform"] = function (request, data, root) {
  var i562 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Transform' )
  var i563 = data
  i562.position = new pc.Vec3( i563[0], i563[1], i563[2] )
  i562.scale = new pc.Vec3( i563[3], i563[4], i563[5] )
  i562.rotation = new pc.Quat(i563[6], i563[7], i563[8], i563[9])
  return i562
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.BoxCollider"] = function (request, data, root) {
  var i564 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.BoxCollider' )
  var i565 = data
  i564.center = new pc.Vec3( i565[0], i565[1], i565[2] )
  i564.size = new pc.Vec3( i565[3], i565[4], i565[5] )
  i564.enabled = !!i565[6]
  i564.isTrigger = !!i565[7]
  request.r(i565[8], i565[9], 0, i564, 'material')
  return i564
}

Deserializers["Project.Gameplay.Units.PlayerView"] = function (request, data, root) {
  var i566 = root || request.c( 'Project.Gameplay.Units.PlayerView' )
  var i567 = data
  request.r(i567[0], i567[1], 0, i566, '_sword')
  request.r(i567[2], i567[3], 0, i566, 'Label')
  request.r(i567[4], i567[5], 0, i566, 'StopPoint')
  request.r(i567[6], i567[7], 0, i566, 'AnchorPoint')
  request.r(i567[8], i567[9], 0, i566, 'VfxPoint')
  request.r(i567[10], i567[11], 0, i566, 'HighlightRing')
  request.r(i567[12], i567[13], 0, i566, 'WarningRing')
  request.r(i567[14], i567[15], 0, i566, 'LegacyAnim')
  var i569 = i567[16]
  var i568 = []
  for(var i = 0; i < i569.length; i += 1) {
    i568.push( request.d('Project.Gameplay.Units.AnimMapping', i569[i + 0]) );
  }
  i566.LegacyClipMap = i568
  i566._previewScale = i567[17]
  i566._previewDuration = i567[18]
  i566._ringPulseAmplitude = i567[19]
  i566._ringPulseSpeed = i567[20]
  return i566
}

Deserializers["Project.Gameplay.Units.AnimMapping"] = function (request, data, root) {
  var i572 = root || request.c( 'Project.Gameplay.Units.AnimMapping' )
  var i573 = data
  i572.StateName = i573[0]
  request.r(i573[1], i573[2], 0, i572, 'Clip')
  i572.Loop = !!i573[3]
  return i572
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Animation"] = function (request, data, root) {
  var i574 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Animation' )
  var i575 = data
  i574.playAutomatically = !!i575[0]
  request.r(i575[1], i575[2], 0, i574, 'clip')
  var i577 = i575[3]
  var i576 = []
  for(var i = 0; i < i577.length; i += 2) {
  request.r(i577[i + 0], i577[i + 1], 2, i576, '')
  }
  i574.clips = i576
  i574.wrapMode = i575[4]
  i574.enabled = !!i575[5]
  return i574
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.GameObject"] = function (request, data, root) {
  var i580 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.GameObject' )
  var i581 = data
  i580.name = i581[0]
  i580.tagId = i581[1]
  i580.enabled = !!i581[2]
  i580.isStatic = !!i581[3]
  i580.layer = i581[4]
  return i580
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer"] = function (request, data, root) {
  var i582 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer' )
  var i583 = data
  request.r(i583[0], i583[1], 0, i582, 'sharedMesh')
  var i585 = i583[2]
  var i584 = []
  for(var i = 0; i < i585.length; i += 2) {
  request.r(i585[i + 0], i585[i + 1], 2, i584, '')
  }
  i582.bones = i584
  i582.updateWhenOffscreen = !!i583[3]
  i582.localBounds = i583[4]
  request.r(i583[5], i583[6], 0, i582, 'rootBone')
  var i587 = i583[7]
  var i586 = []
  for(var i = 0; i < i587.length; i += 1) {
    i586.push( request.d('Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer+BlendShapeWeight', i587[i + 0]) );
  }
  i582.blendShapesWeights = i586
  i582.enabled = !!i583[8]
  request.r(i583[9], i583[10], 0, i582, 'sharedMaterial')
  var i589 = i583[11]
  var i588 = []
  for(var i = 0; i < i589.length; i += 2) {
  request.r(i589[i + 0], i589[i + 1], 2, i588, '')
  }
  i582.sharedMaterials = i588
  i582.receiveShadows = !!i583[12]
  i582.shadowCastingMode = i583[13]
  i582.sortingLayerID = i583[14]
  i582.sortingOrder = i583[15]
  i582.lightmapIndex = i583[16]
  i582.lightmapSceneIndex = i583[17]
  i582.lightmapScaleOffset = new pc.Vec4( i583[18], i583[19], i583[20], i583[21] )
  i582.lightProbeUsage = i583[22]
  i582.reflectionProbeUsage = i583[23]
  return i582
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer+BlendShapeWeight"] = function (request, data, root) {
  var i594 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer+BlendShapeWeight' )
  var i595 = data
  i594.weight = i595[0]
  return i594
}

Deserializers["Project.Gameplay.Units.PowerLabel"] = function (request, data, root) {
  var i598 = root || request.c( 'Project.Gameplay.Units.PowerLabel' )
  var i599 = data
  request.r(i599[0], i599[1], 0, i598, '_text')
  request.r(i599[2], i599[3], 0, i598, '_background')
  i598._playerColor = new pc.Color(i599[4], i599[5], i599[6], i599[7])
  i598._enemyColor = new pc.Color(i599[8], i599[9], i599[10], i599[11])
  i598._chestColor = new pc.Color(i599[12], i599[13], i599[14], i599[15])
  return i598
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.RectTransform"] = function (request, data, root) {
  var i600 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.RectTransform' )
  var i601 = data
  i600.pivot = new pc.Vec2( i601[0], i601[1] )
  i600.anchorMin = new pc.Vec2( i601[2], i601[3] )
  i600.anchorMax = new pc.Vec2( i601[4], i601[5] )
  i600.sizeDelta = new pc.Vec2( i601[6], i601[7] )
  i600.anchoredPosition3D = new pc.Vec3( i601[8], i601[9], i601[10] )
  i600.rotation = new pc.Quat(i601[11], i601[12], i601[13], i601[14])
  i600.scale = new pc.Vec3( i601[15], i601[16], i601[17] )
  return i600
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Canvas"] = function (request, data, root) {
  var i602 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Canvas' )
  var i603 = data
  i602.planeDistance = i603[0]
  i602.referencePixelsPerUnit = i603[1]
  i602.isFallbackOverlay = !!i603[2]
  i602.renderMode = i603[3]
  i602.renderOrder = i603[4]
  i602.sortingLayerName = i603[5]
  i602.sortingOrder = i603[6]
  i602.scaleFactor = i603[7]
  request.r(i603[8], i603[9], 0, i602, 'worldCamera')
  i602.overrideSorting = !!i603[10]
  i602.pixelPerfect = !!i603[11]
  i602.targetDisplay = i603[12]
  i602.overridePixelPerfect = !!i603[13]
  i602.enabled = !!i603[14]
  return i602
}

Deserializers["UnityEngine.UI.CanvasScaler"] = function (request, data, root) {
  var i604 = root || request.c( 'UnityEngine.UI.CanvasScaler' )
  var i605 = data
  i604.m_UiScaleMode = i605[0]
  i604.m_ReferencePixelsPerUnit = i605[1]
  i604.m_ScaleFactor = i605[2]
  i604.m_ReferenceResolution = new pc.Vec2( i605[3], i605[4] )
  i604.m_ScreenMatchMode = i605[5]
  i604.m_MatchWidthOrHeight = i605[6]
  i604.m_PhysicalUnit = i605[7]
  i604.m_FallbackScreenDPI = i605[8]
  i604.m_DefaultSpriteDPI = i605[9]
  i604.m_DynamicPixelsPerUnit = i605[10]
  i604.m_PresetInfoIsWorld = !!i605[11]
  return i604
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer"] = function (request, data, root) {
  var i606 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer' )
  var i607 = data
  i606.cullTransparentMesh = !!i607[0]
  return i606
}

Deserializers["UnityEngine.UI.Image"] = function (request, data, root) {
  var i608 = root || request.c( 'UnityEngine.UI.Image' )
  var i609 = data
  request.r(i609[0], i609[1], 0, i608, 'm_Sprite')
  i608.m_Type = i609[2]
  i608.m_PreserveAspect = !!i609[3]
  i608.m_FillCenter = !!i609[4]
  i608.m_FillMethod = i609[5]
  i608.m_FillAmount = i609[6]
  i608.m_FillClockwise = !!i609[7]
  i608.m_FillOrigin = i609[8]
  i608.m_UseSpriteMesh = !!i609[9]
  i608.m_PixelsPerUnitMultiplier = i609[10]
  request.r(i609[11], i609[12], 0, i608, 'm_Material')
  i608.m_Maskable = !!i609[13]
  i608.m_Color = new pc.Color(i609[14], i609[15], i609[16], i609[17])
  i608.m_RaycastTarget = !!i609[18]
  i608.m_RaycastPadding = new pc.Vec4( i609[19], i609[20], i609[21], i609[22] )
  return i608
}

Deserializers["UnityEngine.UI.Text"] = function (request, data, root) {
  var i610 = root || request.c( 'UnityEngine.UI.Text' )
  var i611 = data
  i610.m_FontData = request.d('UnityEngine.UI.FontData', i611[0], i610.m_FontData)
  i610.m_Text = i611[1]
  request.r(i611[2], i611[3], 0, i610, 'm_Material')
  i610.m_Maskable = !!i611[4]
  i610.m_Color = new pc.Color(i611[5], i611[6], i611[7], i611[8])
  i610.m_RaycastTarget = !!i611[9]
  i610.m_RaycastPadding = new pc.Vec4( i611[10], i611[11], i611[12], i611[13] )
  return i610
}

Deserializers["UnityEngine.UI.FontData"] = function (request, data, root) {
  var i612 = root || request.c( 'UnityEngine.UI.FontData' )
  var i613 = data
  request.r(i613[0], i613[1], 0, i612, 'm_Font')
  i612.m_FontSize = i613[2]
  i612.m_FontStyle = i613[3]
  i612.m_BestFit = !!i613[4]
  i612.m_MinSize = i613[5]
  i612.m_MaxSize = i613[6]
  i612.m_Alignment = i613[7]
  i612.m_AlignByGeometry = !!i613[8]
  i612.m_RichText = !!i613[9]
  i612.m_HorizontalOverflow = i613[10]
  i612.m_VerticalOverflow = i613[11]
  i612.m_LineSpacing = i613[12]
  return i612
}

Deserializers["Project.Gameplay.Vfx.FootstepEmitter"] = function (request, data, root) {
  var i614 = root || request.c( 'Project.Gameplay.Vfx.FootstepEmitter' )
  var i615 = data
  request.r(i615[0], i615[1], 0, i614, '_footSprite')
  i614._emitDistance = i615[2]
  i614._sideOffset = i615[3]
  i614._lifetime = i615[4]
  i614._scale = i615[5]
  i614._color = new pc.Color(i615[6], i615[7], i615[8], i615[9])
  i614._yOffset = i615[10]
  i614._minStepSpeed = i615[11]
  return i614
}

Deserializers["Project.Gameplay.Units.ChestView"] = function (request, data, root) {
  var i616 = root || request.c( 'Project.Gameplay.Units.ChestView' )
  var i617 = data
  request.r(i617[0], i617[1], 0, i616, '_flyingSword')
  i616._idleBobAmplitude = i617[2]
  i616._idleBobSpeed = i617[3]
  request.r(i617[4], i617[5], 0, i616, 'Label')
  request.r(i617[6], i617[7], 0, i616, 'StopPoint')
  request.r(i617[8], i617[9], 0, i616, 'AnchorPoint')
  request.r(i617[10], i617[11], 0, i616, 'VfxPoint')
  request.r(i617[12], i617[13], 0, i616, 'HighlightRing')
  request.r(i617[14], i617[15], 0, i616, 'WarningRing')
  request.r(i617[16], i617[17], 0, i616, 'LegacyAnim')
  var i619 = i617[18]
  var i618 = []
  for(var i = 0; i < i619.length; i += 1) {
    i618.push( request.d('Project.Gameplay.Units.AnimMapping', i619[i + 0]) );
  }
  i616.LegacyClipMap = i618
  i616._previewScale = i617[19]
  i616._previewDuration = i617[20]
  i616._ringPulseAmplitude = i617[21]
  i616._ringPulseSpeed = i617[22]
  return i616
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.MeshFilter"] = function (request, data, root) {
  var i620 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.MeshFilter' )
  var i621 = data
  request.r(i621[0], i621[1], 0, i620, 'sharedMesh')
  return i620
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.MeshRenderer"] = function (request, data, root) {
  var i622 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.MeshRenderer' )
  var i623 = data
  request.r(i623[0], i623[1], 0, i622, 'additionalVertexStreams')
  i622.enabled = !!i623[2]
  request.r(i623[3], i623[4], 0, i622, 'sharedMaterial')
  var i625 = i623[5]
  var i624 = []
  for(var i = 0; i < i625.length; i += 2) {
  request.r(i625[i + 0], i625[i + 1], 2, i624, '')
  }
  i622.sharedMaterials = i624
  i622.receiveShadows = !!i623[6]
  i622.shadowCastingMode = i623[7]
  i622.sortingLayerID = i623[8]
  i622.sortingOrder = i623[9]
  i622.lightmapIndex = i623[10]
  i622.lightmapSceneIndex = i623[11]
  i622.lightmapScaleOffset = new pc.Vec4( i623[12], i623[13], i623[14], i623[15] )
  i622.lightProbeUsage = i623[16]
  i622.reflectionProbeUsage = i623[17]
  return i622
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CapsuleCollider"] = function (request, data, root) {
  var i626 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CapsuleCollider' )
  var i627 = data
  i626.center = new pc.Vec3( i627[0], i627[1], i627[2] )
  i626.radius = i627[3]
  i626.height = i627[4]
  i626.direction = i627[5]
  i626.enabled = !!i627[6]
  i626.isTrigger = !!i627[7]
  request.r(i627[8], i627[9], 0, i626, 'material')
  return i626
}

Deserializers["Project.Gameplay.Units.EnemyView"] = function (request, data, root) {
  var i628 = root || request.c( 'Project.Gameplay.Units.EnemyView' )
  var i629 = data
  request.r(i629[0], i629[1], 0, i628, 'Label')
  request.r(i629[2], i629[3], 0, i628, 'StopPoint')
  request.r(i629[4], i629[5], 0, i628, 'AnchorPoint')
  request.r(i629[6], i629[7], 0, i628, 'VfxPoint')
  request.r(i629[8], i629[9], 0, i628, 'HighlightRing')
  request.r(i629[10], i629[11], 0, i628, 'WarningRing')
  request.r(i629[12], i629[13], 0, i628, 'LegacyAnim')
  var i631 = i629[14]
  var i630 = []
  for(var i = 0; i < i631.length; i += 1) {
    i630.push( request.d('Project.Gameplay.Units.AnimMapping', i631[i + 0]) );
  }
  i628.LegacyClipMap = i630
  i628._previewScale = i629[15]
  i628._previewDuration = i629[16]
  i628._ringPulseAmplitude = i629[17]
  i628._ringPulseSpeed = i629[18]
  return i628
}

Deserializers["Project.Gameplay.Vfx.FloatingNumber"] = function (request, data, root) {
  var i632 = root || request.c( 'Project.Gameplay.Vfx.FloatingNumber' )
  var i633 = data
  request.r(i633[0], i633[1], 0, i632, '_text')
  request.r(i633[2], i633[3], 0, i632, '_group')
  i632._arcHeight = i633[4]
  return i632
}

Deserializers["UnityEngine.UI.GraphicRaycaster"] = function (request, data, root) {
  var i634 = root || request.c( 'UnityEngine.UI.GraphicRaycaster' )
  var i635 = data
  i634.m_IgnoreReversedGraphics = !!i635[0]
  i634.m_BlockingObjects = i635[1]
  i634.m_BlockingMask = UnityEngine.LayerMask.FromIntegerValue( i635[2] )
  return i634
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasGroup"] = function (request, data, root) {
  var i636 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasGroup' )
  var i637 = data
  i636.m_Alpha = i637[0]
  i636.m_Interactable = !!i637[1]
  i636.m_BlocksRaycasts = !!i637[2]
  i636.m_IgnoreParentGroups = !!i637[3]
  i636.enabled = !!i637[4]
  return i636
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer"] = function (request, data, root) {
  var i638 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer' )
  var i639 = data
  i638.color = new pc.Color(i639[0], i639[1], i639[2], i639[3])
  request.r(i639[4], i639[5], 0, i638, 'sprite')
  i638.flipX = !!i639[6]
  i638.flipY = !!i639[7]
  i638.drawMode = i639[8]
  i638.size = new pc.Vec2( i639[9], i639[10] )
  i638.tileMode = i639[11]
  i638.adaptiveModeThreshold = i639[12]
  i638.maskInteraction = i639[13]
  i638.spriteSortPoint = i639[14]
  i638.enabled = !!i639[15]
  request.r(i639[16], i639[17], 0, i638, 'sharedMaterial')
  var i641 = i639[18]
  var i640 = []
  for(var i = 0; i < i641.length; i += 2) {
  request.r(i641[i + 0], i641[i + 1], 2, i640, '')
  }
  i638.sharedMaterials = i640
  i638.receiveShadows = !!i639[19]
  i638.shadowCastingMode = i639[20]
  i638.sortingLayerID = i639[21]
  i638.sortingOrder = i639[22]
  i638.lightmapIndex = i639[23]
  i638.lightmapSceneIndex = i639[24]
  i638.lightmapScaleOffset = new pc.Vec4( i639[25], i639[26], i639[27], i639[28] )
  i638.lightProbeUsage = i639[29]
  i638.reflectionProbeUsage = i639[30]
  return i638
}

Deserializers["Project.Gameplay.Vfx.SpriteSequencePlayer"] = function (request, data, root) {
  var i642 = root || request.c( 'Project.Gameplay.Vfx.SpriteSequencePlayer' )
  var i643 = data
  request.r(i643[0], i643[1], 0, i642, '_renderer')
  var i645 = i643[2]
  var i644 = []
  for(var i = 0; i < i645.length; i += 2) {
  request.r(i645[i + 0], i645[i + 1], 2, i644, '')
  }
  i642._frames = i644
  i642._fps = i643[3]
  i642._loop = !!i643[4]
  i642._disableOnFinish = !!i643[5]
  i642._billboardToCamera = !!i643[6]
  return i642
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.Scene"] = function (request, data, root) {
  var i648 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.Scene' )
  var i649 = data
  i648.name = i649[0]
  i648.index = i649[1]
  i648.startup = !!i649[2]
  return i648
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Camera"] = function (request, data, root) {
  var i650 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Camera' )
  var i651 = data
  i650.aspect = i651[0]
  i650.orthographic = !!i651[1]
  i650.orthographicSize = i651[2]
  i650.backgroundColor = new pc.Color(i651[3], i651[4], i651[5], i651[6])
  i650.nearClipPlane = i651[7]
  i650.farClipPlane = i651[8]
  i650.fieldOfView = i651[9]
  i650.depth = i651[10]
  i650.clearFlags = i651[11]
  i650.cullingMask = i651[12]
  i650.rect = i651[13]
  request.r(i651[14], i651[15], 0, i650, 'targetTexture')
  i650.usePhysicalProperties = !!i651[16]
  i650.focalLength = i651[17]
  i650.sensorSize = new pc.Vec2( i651[18], i651[19] )
  i650.lensShift = new pc.Vec2( i651[20], i651[21] )
  i650.gateFit = i651[22]
  i650.commandBufferCount = i651[23]
  i650.cameraType = i651[24]
  i650.enabled = !!i651[25]
  return i650
}

Deserializers["Project.Gameplay.CameraFx.ScreenShake"] = function (request, data, root) {
  var i652 = root || request.c( 'Project.Gameplay.CameraFx.ScreenShake' )
  var i653 = data
  return i652
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Light"] = function (request, data, root) {
  var i654 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Light' )
  var i655 = data
  i654.type = i655[0]
  i654.color = new pc.Color(i655[1], i655[2], i655[3], i655[4])
  i654.cullingMask = i655[5]
  i654.intensity = i655[6]
  i654.range = i655[7]
  i654.spotAngle = i655[8]
  i654.shadows = i655[9]
  i654.shadowNormalBias = i655[10]
  i654.shadowBias = i655[11]
  i654.shadowStrength = i655[12]
  i654.shadowResolution = i655[13]
  i654.lightmapBakeType = i655[14]
  i654.renderMode = i655[15]
  request.r(i655[16], i655[17], 0, i654, 'cookie')
  i654.cookieSize = i655[18]
  i654.shadowNearPlane = i655[19]
  i654.occlusionMaskChannel = i655[20]
  i654.isBaked = !!i655[21]
  i654.mixedLightingMode = i655[22]
  i654.enabled = !!i655[23]
  return i654
}

Deserializers["Project.Gameplay.UnitSpawner"] = function (request, data, root) {
  var i656 = root || request.c( 'Project.Gameplay.UnitSpawner' )
  var i657 = data
  request.r(i657[0], i657[1], 0, i656, '_bank')
  request.r(i657[2], i657[3], 0, i656, '_root')
  i656._spawnRotation = new pc.Vec3( i657[4], i657[5], i657[6] )
  return i656
}

Deserializers["Project.Gameplay.Flow.BattleFlow"] = function (request, data, root) {
  var i658 = root || request.c( 'Project.Gameplay.Flow.BattleFlow' )
  var i659 = data
  request.r(i659[0], i659[1], 0, i658, '_input')
  request.r(i659[2], i659[3], 0, i658, '_indicator')
  request.r(i659[4], i659[5], 0, i658, '_vfx')
  request.r(i659[6], i659[7], 0, i658, '_shake')
  request.r(i659[8], i659[9], 0, i658, '_floatingNumberPrefab')
  request.r(i659[10], i659[11], 0, i658, '_floatingNumbersRoot')
  i658._enemyArrowColor = new pc.Color(i659[12], i659[13], i659[14], i659[15])
  i658._winnableArrowColor = new pc.Color(i659[16], i659[17], i659[18], i659[19])
  i658._chestArrowColor = new pc.Color(i659[20], i659[21], i659[22], i659[23])
  return i658
}

Deserializers["Project.Gameplay.Targeting.TapInput"] = function (request, data, root) {
  var i660 = root || request.c( 'Project.Gameplay.Targeting.TapInput' )
  var i661 = data
  request.r(i661[0], i661[1], 0, i660, '_camera')
  i660._tappableMask = UnityEngine.LayerMask.FromIntegerValue( i661[2] )
  return i660
}

Deserializers["Project.Gameplay.Vfx.VfxService"] = function (request, data, root) {
  var i662 = root || request.c( 'Project.Gameplay.Vfx.VfxService' )
  var i663 = data
  request.r(i663[0], i663[1], 0, i662, '_bank')
  i662._autoDestroy = i663[2]
  return i662
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.LineRenderer"] = function (request, data, root) {
  var i664 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.LineRenderer' )
  var i665 = data
  i664.textureMode = i665[0]
  i664.alignment = i665[1]
  i664.widthCurve = new pc.AnimationCurve( { keys_flow: i665[2] } )
  i664.colorGradient = i665[3] ? new pc.ColorGradient(i665[3][0], i665[3][1], i665[3][2]) : null
  var i667 = i665[4]
  var i666 = []
  for(var i = 0; i < i667.length; i += 3) {
    i666.push( new pc.Vec3( i667[i + 0], i667[i + 1], i667[i + 2] ) );
  }
  i664.positions = i666
  i664.positionCount = i665[5]
  i664.widthMultiplier = i665[6]
  i664.startWidth = i665[7]
  i664.endWidth = i665[8]
  i664.numCornerVertices = i665[9]
  i664.numCapVertices = i665[10]
  i664.useWorldSpace = !!i665[11]
  i664.loop = !!i665[12]
  i664.startColor = new pc.Color(i665[13], i665[14], i665[15], i665[16])
  i664.endColor = new pc.Color(i665[17], i665[18], i665[19], i665[20])
  i664.generateLightingData = !!i665[21]
  i664.enabled = !!i665[22]
  request.r(i665[23], i665[24], 0, i664, 'sharedMaterial')
  var i669 = i665[25]
  var i668 = []
  for(var i = 0; i < i669.length; i += 2) {
  request.r(i669[i + 0], i669[i + 1], 2, i668, '')
  }
  i664.sharedMaterials = i668
  i664.receiveShadows = !!i665[26]
  i664.shadowCastingMode = i665[27]
  i664.sortingLayerID = i665[28]
  i664.sortingOrder = i665[29]
  i664.lightmapIndex = i665[30]
  i664.lightmapSceneIndex = i665[31]
  i664.lightmapScaleOffset = new pc.Vec4( i665[32], i665[33], i665[34], i665[35] )
  i664.lightProbeUsage = i665[36]
  i664.reflectionProbeUsage = i665[37]
  return i664
}

Deserializers["Project.Gameplay.Targeting.TargetIndicator"] = function (request, data, root) {
  var i672 = root || request.c( 'Project.Gameplay.Targeting.TargetIndicator' )
  var i673 = data
  request.r(i673[0], i673[1], 0, i672, '_line')
  request.r(i673[2], i673[3], 0, i672, '_selectionGlow')
  i672._heightOffset = i673[4]
  i672._dashWorldLength = i673[5]
  i672._flowSpeed = i673[6]
  i672._color = new pc.Color(i673[7], i673[8], i673[9], i673[10])
  return i672
}

Deserializers["Project.Audio.AudioService"] = function (request, data, root) {
  var i674 = root || request.c( 'Project.Audio.AudioService' )
  var i675 = data
  request.r(i675[0], i675[1], 0, i674, '_root')
  request.r(i675[2], i675[3], 0, i674, '_bank')
  request.r(i675[4], i675[5], 0, i674, '_music')
  var i677 = i675[6]
  var i676 = []
  for(var i = 0; i < i677.length; i += 2) {
  request.r(i677[i + 0], i677[i + 1], 2, i676, '')
  }
  i674._sfxPool = i676
  return i674
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.AudioSource"] = function (request, data, root) {
  var i680 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.AudioSource' )
  var i681 = data
  request.r(i681[0], i681[1], 0, i680, 'clip')
  request.r(i681[2], i681[3], 0, i680, 'outputAudioMixerGroup')
  i680.playOnAwake = !!i681[4]
  i680.loop = !!i681[5]
  i680.time = i681[6]
  i680.volume = i681[7]
  i680.pitch = i681[8]
  i680.enabled = !!i681[9]
  return i680
}

Deserializers["Project.Integration.MraidBridge"] = function (request, data, root) {
  var i682 = root || request.c( 'Project.Integration.MraidBridge' )
  var i683 = data
  request.r(i683[0], i683[1], 0, i682, '_root')
  request.r(i683[2], i683[3], 0, i682, '_audio')
  i682._ctaUrl = i683[4]
  return i682
}

Deserializers["Project.Integration.AnalyticsService"] = function (request, data, root) {
  var i684 = root || request.c( 'Project.Integration.AnalyticsService' )
  var i685 = data
  request.r(i685[0], i685[1], 0, i684, '_root')
  return i684
}

Deserializers["UnityEngine.UI.AspectRatioFitter"] = function (request, data, root) {
  var i686 = root || request.c( 'UnityEngine.UI.AspectRatioFitter' )
  var i687 = data
  i686.m_AspectMode = i687[0]
  i686.m_AspectRatio = i687[1]
  return i686
}

Deserializers["Project.UI.Hud.HudView"] = function (request, data, root) {
  var i688 = root || request.c( 'Project.UI.Hud.HudView' )
  var i689 = data
  request.r(i689[0], i689[1], 0, i688, '_playerPower')
  request.r(i689[2], i689[3], 0, i688, '_hint')
  return i688
}

Deserializers["Project.UI.Hud.HudPresenter"] = function (request, data, root) {
  var i690 = root || request.c( 'Project.UI.Hud.HudPresenter' )
  var i691 = data
  request.r(i691[0], i691[1], 0, i690, '_root')
  request.r(i691[2], i691[3], 0, i690, '_view')
  return i690
}

Deserializers["UnityEngine.UI.Button"] = function (request, data, root) {
  var i692 = root || request.c( 'UnityEngine.UI.Button' )
  var i693 = data
  i692.m_OnClick = request.d('UnityEngine.UI.Button+ButtonClickedEvent', i693[0], i692.m_OnClick)
  i692.m_Navigation = request.d('UnityEngine.UI.Navigation', i693[1], i692.m_Navigation)
  i692.m_Transition = i693[2]
  i692.m_Colors = request.d('UnityEngine.UI.ColorBlock', i693[3], i692.m_Colors)
  i692.m_SpriteState = request.d('UnityEngine.UI.SpriteState', i693[4], i692.m_SpriteState)
  i692.m_AnimationTriggers = request.d('UnityEngine.UI.AnimationTriggers', i693[5], i692.m_AnimationTriggers)
  i692.m_Interactable = !!i693[6]
  request.r(i693[7], i693[8], 0, i692, 'm_TargetGraphic')
  return i692
}

Deserializers["UnityEngine.UI.Button+ButtonClickedEvent"] = function (request, data, root) {
  var i694 = root || request.c( 'UnityEngine.UI.Button+ButtonClickedEvent' )
  var i695 = data
  i694.m_PersistentCalls = request.d('UnityEngine.Events.PersistentCallGroup', i695[0], i694.m_PersistentCalls)
  return i694
}

Deserializers["UnityEngine.Events.PersistentCallGroup"] = function (request, data, root) {
  var i696 = root || request.c( 'UnityEngine.Events.PersistentCallGroup' )
  var i697 = data
  var i699 = i697[0]
  var i698 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Events.PersistentCall')))
  for(var i = 0; i < i699.length; i += 1) {
    i698.add(request.d('UnityEngine.Events.PersistentCall', i699[i + 0]));
  }
  i696.m_Calls = i698
  return i696
}

Deserializers["UnityEngine.Events.PersistentCall"] = function (request, data, root) {
  var i702 = root || request.c( 'UnityEngine.Events.PersistentCall' )
  var i703 = data
  request.r(i703[0], i703[1], 0, i702, 'm_Target')
  i702.m_TargetAssemblyTypeName = i703[2]
  i702.m_MethodName = i703[3]
  i702.m_Mode = i703[4]
  i702.m_Arguments = request.d('UnityEngine.Events.ArgumentCache', i703[5], i702.m_Arguments)
  i702.m_CallState = i703[6]
  return i702
}

Deserializers["UnityEngine.UI.Navigation"] = function (request, data, root) {
  var i704 = root || request.c( 'UnityEngine.UI.Navigation' )
  var i705 = data
  i704.m_Mode = i705[0]
  i704.m_WrapAround = !!i705[1]
  request.r(i705[2], i705[3], 0, i704, 'm_SelectOnUp')
  request.r(i705[4], i705[5], 0, i704, 'm_SelectOnDown')
  request.r(i705[6], i705[7], 0, i704, 'm_SelectOnLeft')
  request.r(i705[8], i705[9], 0, i704, 'm_SelectOnRight')
  return i704
}

Deserializers["UnityEngine.UI.ColorBlock"] = function (request, data, root) {
  var i706 = root || request.c( 'UnityEngine.UI.ColorBlock' )
  var i707 = data
  i706.m_NormalColor = new pc.Color(i707[0], i707[1], i707[2], i707[3])
  i706.m_HighlightedColor = new pc.Color(i707[4], i707[5], i707[6], i707[7])
  i706.m_PressedColor = new pc.Color(i707[8], i707[9], i707[10], i707[11])
  i706.m_SelectedColor = new pc.Color(i707[12], i707[13], i707[14], i707[15])
  i706.m_DisabledColor = new pc.Color(i707[16], i707[17], i707[18], i707[19])
  i706.m_ColorMultiplier = i707[20]
  i706.m_FadeDuration = i707[21]
  return i706
}

Deserializers["UnityEngine.UI.SpriteState"] = function (request, data, root) {
  var i708 = root || request.c( 'UnityEngine.UI.SpriteState' )
  var i709 = data
  request.r(i709[0], i709[1], 0, i708, 'm_HighlightedSprite')
  request.r(i709[2], i709[3], 0, i708, 'm_PressedSprite')
  request.r(i709[4], i709[5], 0, i708, 'm_SelectedSprite')
  request.r(i709[6], i709[7], 0, i708, 'm_DisabledSprite')
  return i708
}

Deserializers["UnityEngine.UI.AnimationTriggers"] = function (request, data, root) {
  var i710 = root || request.c( 'UnityEngine.UI.AnimationTriggers' )
  var i711 = data
  i710.m_NormalTrigger = i711[0]
  i710.m_HighlightedTrigger = i711[1]
  i710.m_PressedTrigger = i711[2]
  i710.m_SelectedTrigger = i711[3]
  i710.m_DisabledTrigger = i711[4]
  return i710
}

Deserializers["Project.UI.Common.MuteToggle"] = function (request, data, root) {
  var i712 = root || request.c( 'Project.UI.Common.MuteToggle' )
  var i713 = data
  request.r(i713[0], i713[1], 0, i712, '_button')
  request.r(i713[2], i713[3], 0, i712, '_icon')
  request.r(i713[4], i713[5], 0, i712, '_onSprite')
  request.r(i713[6], i713[7], 0, i712, '_offSprite')
  i712._startMuted = !!i713[8]
  return i712
}

Deserializers["Project.UI.Common.TutorialNudge"] = function (request, data, root) {
  var i714 = root || request.c( 'Project.UI.Common.TutorialNudge' )
  var i715 = data
  request.r(i715[0], i715[1], 0, i714, '_root')
  request.r(i715[2], i715[3], 0, i714, '_group')
  request.r(i715[4], i715[5], 0, i714, '_arrow')
  i714._idleSeconds = i715[6]
  i714._fadeDuration = i715[7]
  i714._arrowBobAmplitude = i715[8]
  i714._arrowBobSpeed = i715[9]
  return i714
}

Deserializers["Project.UI.Common.MobCounter"] = function (request, data, root) {
  var i716 = root || request.c( 'Project.UI.Common.MobCounter' )
  var i717 = data
  request.r(i717[0], i717[1], 0, i716, '_root')
  request.r(i717[2], i717[3], 0, i716, '_label')
  request.r(i717[4], i717[5], 0, i716, '_group')
  i716._format = i717[6]
  return i716
}

Deserializers["Project.UI.Common.TouchRipple"] = function (request, data, root) {
  var i718 = root || request.c( 'Project.UI.Common.TouchRipple' )
  var i719 = data
  request.r(i719[0], i719[1], 0, i718, '_canvasRect')
  request.r(i719[2], i719[3], 0, i718, '_camera')
  var i721 = i719[4]
  var i720 = []
  for(var i = 0; i < i721.length; i += 2) {
  request.r(i721[i + 0], i721[i + 1], 2, i720, '')
  }
  i718._poolRects = i720
  var i723 = i719[5]
  var i722 = []
  for(var i = 0; i < i723.length; i += 2) {
  request.r(i723[i + 0], i723[i + 1], 2, i722, '')
  }
  i718._poolImages = i722
  i718._maxScale = i719[6]
  i718._duration = i719[7]
  i718._color = new pc.Color(i719[8], i719[9], i719[10], i719[11])
  return i718
}

Deserializers["Project.UI.EndCard.EndCardView"] = function (request, data, root) {
  var i728 = root || request.c( 'Project.UI.EndCard.EndCardView' )
  var i729 = data
  request.r(i729[0], i729[1], 0, i728, '_group')
  request.r(i729[2], i729[3], 0, i728, '_ctaButton')
  request.r(i729[4], i729[5], 0, i728, '_retryButton')
  request.r(i729[6], i729[7], 0, i728, '_skipButton')
  request.r(i729[8], i729[9], 0, i728, '_title')
  request.r(i729[10], i729[11], 0, i728, '_subtitle')
  var i731 = i729[12]
  var i730 = []
  for(var i = 0; i < i731.length; i += 2) {
  request.r(i731[i + 0], i731[i + 1], 2, i730, '')
  }
  i728._stars = i730
  request.r(i729[13], i729[14], 0, i728, '_starFilledSprite')
  request.r(i729[15], i729[16], 0, i728, '_starOutlineSprite')
  i728._starFilledColor = new pc.Color(i729[17], i729[18], i729[19], i729[20])
  i728._starEmptyColor = new pc.Color(i729[21], i729[22], i729[23], i729[24])
  i728._ctaPulseAmplitude = i729[25]
  i728._ctaPulseSpeed = i729[26]
  return i728
}

Deserializers["Project.UI.EndCard.EndCardPresenter"] = function (request, data, root) {
  var i732 = root || request.c( 'Project.UI.EndCard.EndCardPresenter' )
  var i733 = data
  request.r(i733[0], i733[1], 0, i732, '_root')
  request.r(i733[2], i733[3], 0, i732, '_view')
  i732._winTitle = i733[4]
  i732._loseTitle = i733[5]
  i732._winSubtitleFormat = i733[6]
  i732._loseSubtitleFormat = i733[7]
  return i732
}

Deserializers["Project.UI.Common.TweenButton"] = function (request, data, root) {
  var i734 = root || request.c( 'Project.UI.Common.TweenButton' )
  var i735 = data
  i734._pressedScale = i735[0]
  i734._duration = i735[1]
  return i734
}

Deserializers["Project.UI.Common.UiSoundButton"] = function (request, data, root) {
  var i736 = root || request.c( 'Project.UI.Common.UiSoundButton' )
  var i737 = data
  request.r(i737[0], i737[1], 0, i736, '_audio')
  request.r(i737[2], i737[3], 0, i736, '_button')
  i736._key = i737[4]
  return i736
}

Deserializers["Project.UI.Common.TapToStartSplash"] = function (request, data, root) {
  var i738 = root || request.c( 'Project.UI.Common.TapToStartSplash' )
  var i739 = data
  request.r(i739[0], i739[1], 0, i738, '_group')
  request.r(i739[2], i739[3], 0, i738, '_tapButton')
  request.r(i739[4], i739[5], 0, i738, '_ctaPulse')
  i738._ctaPulseAmplitude = i739[6]
  i738._ctaPulseSpeed = i739[7]
  i738._fadeDuration = i739[8]
  return i738
}

Deserializers["UnityEngine.EventSystems.EventSystem"] = function (request, data, root) {
  var i740 = root || request.c( 'UnityEngine.EventSystems.EventSystem' )
  var i741 = data
  request.r(i741[0], i741[1], 0, i740, 'm_FirstSelected')
  i740.m_sendNavigationEvents = !!i741[2]
  i740.m_DragThreshold = i741[3]
  return i740
}

Deserializers["UnityEngine.EventSystems.StandaloneInputModule"] = function (request, data, root) {
  var i742 = root || request.c( 'UnityEngine.EventSystems.StandaloneInputModule' )
  var i743 = data
  i742.m_HorizontalAxis = i743[0]
  i742.m_VerticalAxis = i743[1]
  i742.m_SubmitButton = i743[2]
  i742.m_CancelButton = i743[3]
  i742.m_InputActionsPerSecond = i743[4]
  i742.m_RepeatDelay = i743[5]
  i742.m_ForceModuleActive = !!i743[6]
  i742.m_SendPointerHoverToParent = !!i743[7]
  return i742
}

Deserializers["Project.Core.GameRoot"] = function (request, data, root) {
  var i744 = root || request.c( 'Project.Core.GameRoot' )
  var i745 = data
  request.r(i745[0], i745[1], 0, i744, '_levelConfig')
  request.r(i745[2], i745[3], 0, i744, '_balanceConfig')
  request.r(i745[4], i745[5], 0, i744, '_spawner')
  request.r(i745[6], i745[7], 0, i744, '_battleFlow')
  request.r(i745[8], i745[9], 0, i744, '_camera')
  return i744
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings"] = function (request, data, root) {
  var i746 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings' )
  var i747 = data
  i746.ambientIntensity = i747[0]
  i746.reflectionIntensity = i747[1]
  i746.ambientMode = i747[2]
  i746.ambientLight = new pc.Color(i747[3], i747[4], i747[5], i747[6])
  i746.ambientSkyColor = new pc.Color(i747[7], i747[8], i747[9], i747[10])
  i746.ambientGroundColor = new pc.Color(i747[11], i747[12], i747[13], i747[14])
  i746.ambientEquatorColor = new pc.Color(i747[15], i747[16], i747[17], i747[18])
  i746.fogColor = new pc.Color(i747[19], i747[20], i747[21], i747[22])
  i746.fogEndDistance = i747[23]
  i746.fogStartDistance = i747[24]
  i746.fogDensity = i747[25]
  i746.fog = !!i747[26]
  request.r(i747[27], i747[28], 0, i746, 'skybox')
  i746.fogMode = i747[29]
  var i749 = i747[30]
  var i748 = []
  for(var i = 0; i < i749.length; i += 1) {
    i748.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap', i749[i + 0]) );
  }
  i746.lightmaps = i748
  i746.lightProbes = request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes', i747[31], i746.lightProbes)
  i746.lightmapsMode = i747[32]
  i746.mixedBakeMode = i747[33]
  i746.environmentLightingMode = i747[34]
  i746.ambientProbe = new pc.SphericalHarmonicsL2(i747[35])
  request.r(i747[36], i747[37], 0, i746, 'customReflection')
  request.r(i747[38], i747[39], 0, i746, 'defaultReflection')
  i746.defaultReflectionMode = i747[40]
  i746.defaultReflectionResolution = i747[41]
  i746.sunLightObjectId = i747[42]
  i746.pixelLightCount = i747[43]
  i746.defaultReflectionHDR = !!i747[44]
  i746.hasLightDataAsset = !!i747[45]
  i746.hasManualGenerate = !!i747[46]
  return i746
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap"] = function (request, data, root) {
  var i752 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap' )
  var i753 = data
  request.r(i753[0], i753[1], 0, i752, 'lightmapColor')
  request.r(i753[2], i753[3], 0, i752, 'lightmapDirection')
  request.r(i753[4], i753[5], 0, i752, 'shadowMask')
  return i752
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes"] = function (request, data, root) {
  var i754 = root || new UnityEngine.LightProbes()
  var i755 = data
  return i754
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader"] = function (request, data, root) {
  var i760 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader' )
  var i761 = data
  var i763 = i761[0]
  var i762 = new (System.Collections.Generic.List$1(Bridge.ns('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError')))
  for(var i = 0; i < i763.length; i += 1) {
    i762.add(request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError', i763[i + 0]));
  }
  i760.ShaderCompilationErrors = i762
  i760.name = i761[1]
  i760.guid = i761[2]
  var i765 = i761[3]
  var i764 = []
  for(var i = 0; i < i765.length; i += 1) {
    i764.push( i765[i + 0] );
  }
  i760.shaderDefinedKeywords = i764
  var i767 = i761[4]
  var i766 = []
  for(var i = 0; i < i767.length; i += 1) {
    i766.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass', i767[i + 0]) );
  }
  i760.passes = i766
  var i769 = i761[5]
  var i768 = []
  for(var i = 0; i < i769.length; i += 1) {
    i768.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass', i769[i + 0]) );
  }
  i760.usePasses = i768
  var i771 = i761[6]
  var i770 = []
  for(var i = 0; i < i771.length; i += 1) {
    i770.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue', i771[i + 0]) );
  }
  i760.defaultParameterValues = i770
  request.r(i761[7], i761[8], 0, i760, 'unityFallbackShader')
  i760.readDepth = !!i761[9]
  i760.hasDepthOnlyPass = !!i761[10]
  i760.isCreatedByShaderGraph = !!i761[11]
  i760.disableBatching = !!i761[12]
  i760.compiled = !!i761[13]
  return i760
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError"] = function (request, data, root) {
  var i774 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError' )
  var i775 = data
  i774.shaderName = i775[0]
  i774.errorMessage = i775[1]
  return i774
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass"] = function (request, data, root) {
  var i780 = root || new pc.UnityShaderPass()
  var i781 = data
  i780.id = i781[0]
  i780.subShaderIndex = i781[1]
  i780.name = i781[2]
  i780.passType = i781[3]
  i780.grabPassTextureName = i781[4]
  i780.usePass = !!i781[5]
  i780.zTest = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[6], i780.zTest)
  i780.zWrite = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[7], i780.zWrite)
  i780.culling = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[8], i780.culling)
  i780.blending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i781[9], i780.blending)
  i780.alphaBlending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i781[10], i780.alphaBlending)
  i780.colorWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[11], i780.colorWriteMask)
  i780.offsetUnits = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[12], i780.offsetUnits)
  i780.offsetFactor = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[13], i780.offsetFactor)
  i780.stencilRef = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[14], i780.stencilRef)
  i780.stencilReadMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[15], i780.stencilReadMask)
  i780.stencilWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i781[16], i780.stencilWriteMask)
  i780.stencilOp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i781[17], i780.stencilOp)
  i780.stencilOpFront = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i781[18], i780.stencilOpFront)
  i780.stencilOpBack = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i781[19], i780.stencilOpBack)
  var i783 = i781[20]
  var i782 = []
  for(var i = 0; i < i783.length; i += 1) {
    i782.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag', i783[i + 0]) );
  }
  i780.tags = i782
  var i785 = i781[21]
  var i784 = []
  for(var i = 0; i < i785.length; i += 1) {
    i784.push( i785[i + 0] );
  }
  i780.passDefinedKeywords = i784
  var i787 = i781[22]
  var i786 = []
  for(var i = 0; i < i787.length; i += 1) {
    i786.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup', i787[i + 0]) );
  }
  i780.passDefinedKeywordGroups = i786
  var i789 = i781[23]
  var i788 = []
  for(var i = 0; i < i789.length; i += 1) {
    i788.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i789[i + 0]) );
  }
  i780.variants = i788
  var i791 = i781[24]
  var i790 = []
  for(var i = 0; i < i791.length; i += 1) {
    i790.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i791[i + 0]) );
  }
  i780.excludedVariants = i790
  i780.hasDepthReader = !!i781[25]
  return i780
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value"] = function (request, data, root) {
  var i792 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value' )
  var i793 = data
  i792.val = i793[0]
  i792.name = i793[1]
  return i792
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending"] = function (request, data, root) {
  var i794 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending' )
  var i795 = data
  i794.src = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i795[0], i794.src)
  i794.dst = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i795[1], i794.dst)
  i794.op = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i795[2], i794.op)
  return i794
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp"] = function (request, data, root) {
  var i796 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp' )
  var i797 = data
  i796.pass = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i797[0], i796.pass)
  i796.fail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i797[1], i796.fail)
  i796.zFail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i797[2], i796.zFail)
  i796.comp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i797[3], i796.comp)
  return i796
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag"] = function (request, data, root) {
  var i800 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag' )
  var i801 = data
  i800.name = i801[0]
  i800.value = i801[1]
  return i800
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup"] = function (request, data, root) {
  var i804 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup' )
  var i805 = data
  var i807 = i805[0]
  var i806 = []
  for(var i = 0; i < i807.length; i += 1) {
    i806.push( i807[i + 0] );
  }
  i804.keywords = i806
  i804.hasDiscard = !!i805[1]
  return i804
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant"] = function (request, data, root) {
  var i810 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant' )
  var i811 = data
  i810.passId = i811[0]
  i810.subShaderIndex = i811[1]
  var i813 = i811[2]
  var i812 = []
  for(var i = 0; i < i813.length; i += 1) {
    i812.push( i813[i + 0] );
  }
  i810.keywords = i812
  i810.vertexProgram = i811[3]
  i810.fragmentProgram = i811[4]
  i810.exportedForWebGl2 = !!i811[5]
  i810.readDepth = !!i811[6]
  return i810
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass"] = function (request, data, root) {
  var i816 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass' )
  var i817 = data
  request.r(i817[0], i817[1], 0, i816, 'shader')
  i816.pass = i817[2]
  return i816
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue"] = function (request, data, root) {
  var i820 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue' )
  var i821 = data
  i820.name = i821[0]
  i820.type = i821[1]
  i820.value = new pc.Vec4( i821[2], i821[3], i821[4], i821[5] )
  i820.textureValue = i821[6]
  i820.shaderPropertyFlag = i821[7]
  return i820
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Sprite"] = function (request, data, root) {
  var i822 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Sprite' )
  var i823 = data
  i822.name = i823[0]
  request.r(i823[1], i823[2], 0, i822, 'texture')
  i822.aabb = i823[3]
  i822.vertices = i823[4]
  i822.triangles = i823[5]
  i822.textureRect = UnityEngine.Rect.MinMaxRect(i823[6], i823[7], i823[8], i823[9])
  i822.packedRect = UnityEngine.Rect.MinMaxRect(i823[10], i823[11], i823[12], i823[13])
  i822.border = new pc.Vec4( i823[14], i823[15], i823[16], i823[17] )
  i822.transparency = i823[18]
  i822.bounds = i823[19]
  i822.pixelsPerUnit = i823[20]
  i822.textureWidth = i823[21]
  i822.textureHeight = i823[22]
  i822.nativeSize = new pc.Vec2( i823[23], i823[24] )
  i822.pivot = new pc.Vec2( i823[25], i823[26] )
  i822.textureRectOffset = new pc.Vec2( i823[27], i823[28] )
  return i822
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.AudioClip"] = function (request, data, root) {
  var i824 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.AudioClip' )
  var i825 = data
  i824.name = i825[0]
  return i824
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip"] = function (request, data, root) {
  var i826 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip' )
  var i827 = data
  i826.name = i827[0]
  i826.wrapMode = i827[1]
  i826.isLooping = !!i827[2]
  i826.length = i827[3]
  var i829 = i827[4]
  var i828 = []
  for(var i = 0; i < i829.length; i += 1) {
    i828.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve', i829[i + 0]) );
  }
  i826.curves = i828
  var i831 = i827[5]
  var i830 = []
  for(var i = 0; i < i831.length; i += 1) {
    i830.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent', i831[i + 0]) );
  }
  i826.events = i830
  i826.halfPrecision = !!i827[6]
  i826._frameRate = i827[7]
  i826.localBounds = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds', i827[8], i826.localBounds)
  i826.hasMuscleCurves = !!i827[9]
  var i833 = i827[10]
  var i832 = []
  for(var i = 0; i < i833.length; i += 1) {
    i832.push( i833[i + 0] );
  }
  i826.clipMuscleConstant = i832
  i826.clipBindingConstant = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant', i827[11], i826.clipBindingConstant)
  return i826
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve"] = function (request, data, root) {
  var i836 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve' )
  var i837 = data
  i836.path = i837[0]
  i836.hash = i837[1]
  i836.componentType = i837[2]
  i836.property = i837[3]
  i836.keys = i837[4]
  var i839 = i837[5]
  var i838 = []
  for(var i = 0; i < i839.length; i += 1) {
    i838.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey', i839[i + 0]) );
  }
  i836.objectReferenceKeys = i838
  return i836
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey"] = function (request, data, root) {
  var i842 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey' )
  var i843 = data
  i842.time = i843[0]
  request.r(i843[1], i843[2], 0, i842, 'value')
  return i842
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent"] = function (request, data, root) {
  var i846 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent' )
  var i847 = data
  i846.functionName = i847[0]
  i846.floatParameter = i847[1]
  i846.intParameter = i847[2]
  i846.stringParameter = i847[3]
  request.r(i847[4], i847[5], 0, i846, 'objectReferenceParameter')
  i846.time = i847[6]
  return i846
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds"] = function (request, data, root) {
  var i848 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds' )
  var i849 = data
  i848.center = new pc.Vec3( i849[0], i849[1], i849[2] )
  i848.extends = new pc.Vec3( i849[3], i849[4], i849[5] )
  return i848
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant"] = function (request, data, root) {
  var i852 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant' )
  var i853 = data
  var i855 = i853[0]
  var i854 = []
  for(var i = 0; i < i855.length; i += 1) {
    i854.push( i855[i + 0] );
  }
  i852.genericBindings = i854
  var i857 = i853[1]
  var i856 = []
  for(var i = 0; i < i857.length; i += 1) {
    i856.push( i857[i + 0] );
  }
  i852.pptrCurveMapping = i856
  return i852
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font"] = function (request, data, root) {
  var i858 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font' )
  var i859 = data
  i858.name = i859[0]
  i858.ascent = i859[1]
  i858.originalLineHeight = i859[2]
  i858.fontSize = i859[3]
  var i861 = i859[4]
  var i860 = []
  for(var i = 0; i < i861.length; i += 1) {
    i860.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo', i861[i + 0]) );
  }
  i858.characterInfo = i860
  request.r(i859[5], i859[6], 0, i858, 'texture')
  i858.originalFontSize = i859[7]
  return i858
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo"] = function (request, data, root) {
  var i864 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo' )
  var i865 = data
  i864.index = i865[0]
  i864.advance = i865[1]
  i864.bearing = i865[2]
  i864.glyphWidth = i865[3]
  i864.glyphHeight = i865[4]
  i864.minX = i865[5]
  i864.maxX = i865[6]
  i864.minY = i865[7]
  i864.maxY = i865[8]
  i864.uvBottomLeftX = i865[9]
  i864.uvBottomLeftY = i865[10]
  i864.uvBottomRightX = i865[11]
  i864.uvBottomRightY = i865[12]
  i864.uvTopLeftX = i865[13]
  i864.uvTopLeftY = i865[14]
  i864.uvTopRightX = i865[15]
  i864.uvTopRightY = i865[16]
  return i864
}

Deserializers["Project.Configs.UnitsBank"] = function (request, data, root) {
  var i866 = root || request.c( 'Project.Configs.UnitsBank' )
  var i867 = data
  request.r(i867[0], i867[1], 0, i866, 'PlayerPrefab')
  request.r(i867[2], i867[3], 0, i866, 'ChestPrefab')
  var i869 = i867[4]
  var i868 = []
  for(var i = 0; i < i869.length; i += 1) {
    i868.push( request.d('Project.Configs.UnitsBank+EnemyEntry', i869[i + 0]) );
  }
  i866.Enemies = i868
  return i866
}

Deserializers["Project.Configs.UnitsBank+EnemyEntry"] = function (request, data, root) {
  var i872 = root || request.c( 'Project.Configs.UnitsBank+EnemyEntry' )
  var i873 = data
  i872.Key = i873[0]
  request.r(i873[1], i873[2], 0, i872, 'Prefab')
  return i872
}

Deserializers["Project.Gameplay.Vfx.VfxBank"] = function (request, data, root) {
  var i874 = root || request.c( 'Project.Gameplay.Vfx.VfxBank' )
  var i875 = data
  var i877 = i875[0]
  var i876 = []
  for(var i = 0; i < i877.length; i += 1) {
    i876.push( request.d('Project.Gameplay.Vfx.VfxBank+Entry', i877[i + 0]) );
  }
  i874.Entries = i876
  return i874
}

Deserializers["Project.Gameplay.Vfx.VfxBank+Entry"] = function (request, data, root) {
  var i880 = root || request.c( 'Project.Gameplay.Vfx.VfxBank+Entry' )
  var i881 = data
  i880.Key = i881[0]
  request.r(i881[1], i881[2], 0, i880, 'Prefab')
  return i880
}

Deserializers["Project.Configs.LevelConfig"] = function (request, data, root) {
  var i882 = root || request.c( 'Project.Configs.LevelConfig' )
  var i883 = data
  i882.Player = request.d('Project.Configs.LevelConfig+PlayerSpawn', i883[0], i882.Player)
  var i885 = i883[1]
  var i884 = new (System.Collections.Generic.List$1(Bridge.ns('Project.Configs.LevelConfig+TargetSpawn')))
  for(var i = 0; i < i885.length; i += 1) {
    i884.add(request.d('Project.Configs.LevelConfig+TargetSpawn', i885[i + 0]));
  }
  i882.Targets = i884
  return i882
}

Deserializers["Project.Configs.LevelConfig+PlayerSpawn"] = function (request, data, root) {
  var i886 = root || request.c( 'Project.Configs.LevelConfig+PlayerSpawn' )
  var i887 = data
  i886.Position = new pc.Vec2( i887[0], i887[1] )
  i886.Power = i887[2]
  return i886
}

Deserializers["Project.Configs.LevelConfig+TargetSpawn"] = function (request, data, root) {
  var i890 = root || request.c( 'Project.Configs.LevelConfig+TargetSpawn' )
  var i891 = data
  i890.Kind = i891[0]
  i890.Position = new pc.Vec2( i891[1], i891[2] )
  i890.Power = i891[3]
  i890.PrefabKey = i891[4]
  return i890
}

Deserializers["Project.Configs.BalanceConfig"] = function (request, data, root) {
  var i892 = root || request.c( 'Project.Configs.BalanceConfig' )
  var i893 = data
  i892.MoveSpeed = i893[0]
  i892.StopDistance = i893[1]
  i892.AttackWindup = i893[2]
  i892.AttackImpactDelay = i893[3]
  i892.AttackRecover = i893[4]
  i892.FailedAttackBounce = i893[5]
  i892.UpgradeDuration = i893[6]
  i892.UpgradeAnimTail = i893[7]
  i892.HitReactionDelay = i893[8]
  i892.DeathAnimDuration = i893[9]
  i892.DeathFadeDuration = i893[10]
  i892.SuperAttackThreshold = i893[11]
  i892.HitShakeAmplitude = i893[12]
  i892.HitShakeDuration = i893[13]
  i892.FloatingNumberRise = i893[14]
  i892.FloatingNumberDuration = i893[15]
  i892.EndCardDelay = i893[16]
  return i892
}

Deserializers["Project.Audio.AudioBank"] = function (request, data, root) {
  var i894 = root || request.c( 'Project.Audio.AudioBank' )
  var i895 = data
  request.r(i895[0], i895[1], 0, i894, 'Music')
  i894.MusicVolume = i895[2]
  var i897 = i895[3]
  var i896 = []
  for(var i = 0; i < i897.length; i += 1) {
    i896.push( request.d('Project.Audio.AudioBank+Sfx', i897[i + 0]) );
  }
  i894.Sfxs = i896
  return i894
}

Deserializers["Project.Audio.AudioBank+Sfx"] = function (request, data, root) {
  var i900 = root || request.c( 'Project.Audio.AudioBank+Sfx' )
  var i901 = data
  i900.Key = i901[0]
  request.r(i901[1], i901[2], 0, i900, 'Clip')
  i900.Volume = i901[3]
  return i900
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources"] = function (request, data, root) {
  var i902 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources' )
  var i903 = data
  var i905 = i903[0]
  var i904 = []
  for(var i = 0; i < i905.length; i += 1) {
    i904.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Resources+File', i905[i + 0]) );
  }
  i902.files = i904
  i902.componentToPrefabIds = i903[1]
  return i902
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources+File"] = function (request, data, root) {
  var i908 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources+File' )
  var i909 = data
  i908.path = i909[0]
  request.r(i909[1], i909[2], 0, i908, 'unityObject')
  return i908
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings"] = function (request, data, root) {
  var i910 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings' )
  var i911 = data
  var i913 = i911[0]
  var i912 = []
  for(var i = 0; i < i913.length; i += 1) {
    i912.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder', i913[i + 0]) );
  }
  i910.scriptsExecutionOrder = i912
  var i915 = i911[1]
  var i914 = []
  for(var i = 0; i < i915.length; i += 1) {
    i914.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer', i915[i + 0]) );
  }
  i910.sortingLayers = i914
  var i917 = i911[2]
  var i916 = []
  for(var i = 0; i < i917.length; i += 1) {
    i916.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer', i917[i + 0]) );
  }
  i910.cullingLayers = i916
  i910.timeSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings', i911[3], i910.timeSettings)
  i910.physicsSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings', i911[4], i910.physicsSettings)
  i910.physics2DSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings', i911[5], i910.physics2DSettings)
  i910.qualitySettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i911[6], i910.qualitySettings)
  i910.enableRealtimeShadows = !!i911[7]
  i910.enableAutoInstancing = !!i911[8]
  i910.enableStaticBatching = !!i911[9]
  i910.enableDynamicBatching = !!i911[10]
  i910.usePreservativeDynamicBatching = !!i911[11]
  i910.lightmapEncodingQuality = i911[12]
  i910.desiredColorSpace = i911[13]
  var i919 = i911[14]
  var i918 = []
  for(var i = 0; i < i919.length; i += 1) {
    i918.push( i919[i + 0] );
  }
  i910.allTags = i918
  return i910
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder"] = function (request, data, root) {
  var i922 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder' )
  var i923 = data
  i922.name = i923[0]
  i922.value = i923[1]
  return i922
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer"] = function (request, data, root) {
  var i926 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer' )
  var i927 = data
  i926.id = i927[0]
  i926.name = i927[1]
  i926.value = i927[2]
  return i926
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer"] = function (request, data, root) {
  var i930 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer' )
  var i931 = data
  i930.id = i931[0]
  i930.name = i931[1]
  return i930
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings"] = function (request, data, root) {
  var i932 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings' )
  var i933 = data
  i932.fixedDeltaTime = i933[0]
  i932.maximumDeltaTime = i933[1]
  i932.timeScale = i933[2]
  i932.maximumParticleTimestep = i933[3]
  return i932
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings"] = function (request, data, root) {
  var i934 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings' )
  var i935 = data
  i934.gravity = new pc.Vec3( i935[0], i935[1], i935[2] )
  i934.defaultSolverIterations = i935[3]
  i934.bounceThreshold = i935[4]
  i934.autoSyncTransforms = !!i935[5]
  i934.autoSimulation = !!i935[6]
  var i937 = i935[7]
  var i936 = []
  for(var i = 0; i < i937.length; i += 1) {
    i936.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask', i937[i + 0]) );
  }
  i934.collisionMatrix = i936
  return i934
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask"] = function (request, data, root) {
  var i940 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask' )
  var i941 = data
  i940.enabled = !!i941[0]
  i940.layerId = i941[1]
  i940.otherLayerId = i941[2]
  return i940
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings"] = function (request, data, root) {
  var i942 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings' )
  var i943 = data
  request.r(i943[0], i943[1], 0, i942, 'material')
  i942.gravity = new pc.Vec2( i943[2], i943[3] )
  i942.positionIterations = i943[4]
  i942.velocityIterations = i943[5]
  i942.velocityThreshold = i943[6]
  i942.maxLinearCorrection = i943[7]
  i942.maxAngularCorrection = i943[8]
  i942.maxTranslationSpeed = i943[9]
  i942.maxRotationSpeed = i943[10]
  i942.baumgarteScale = i943[11]
  i942.baumgarteTOIScale = i943[12]
  i942.timeToSleep = i943[13]
  i942.linearSleepTolerance = i943[14]
  i942.angularSleepTolerance = i943[15]
  i942.defaultContactOffset = i943[16]
  i942.autoSimulation = !!i943[17]
  i942.queriesHitTriggers = !!i943[18]
  i942.queriesStartInColliders = !!i943[19]
  i942.callbacksOnDisable = !!i943[20]
  i942.reuseCollisionCallbacks = !!i943[21]
  i942.autoSyncTransforms = !!i943[22]
  var i945 = i943[23]
  var i944 = []
  for(var i = 0; i < i945.length; i += 1) {
    i944.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask', i945[i + 0]) );
  }
  i942.collisionMatrix = i944
  return i942
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask"] = function (request, data, root) {
  var i948 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask' )
  var i949 = data
  i948.enabled = !!i949[0]
  i948.layerId = i949[1]
  i948.otherLayerId = i949[2]
  return i948
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.QualitySettings"] = function (request, data, root) {
  var i950 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.QualitySettings' )
  var i951 = data
  var i953 = i951[0]
  var i952 = []
  for(var i = 0; i < i953.length; i += 1) {
    i952.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i953[i + 0]) );
  }
  i950.qualityLevels = i952
  var i955 = i951[1]
  var i954 = []
  for(var i = 0; i < i955.length; i += 1) {
    i954.push( i955[i + 0] );
  }
  i950.names = i954
  i950.shadows = i951[2]
  i950.anisotropicFiltering = i951[3]
  i950.antiAliasing = i951[4]
  i950.lodBias = i951[5]
  i950.shadowCascades = i951[6]
  i950.shadowDistance = i951[7]
  i950.shadowmaskMode = i951[8]
  i950.shadowProjection = i951[9]
  i950.shadowResolution = i951[10]
  i950.softParticles = !!i951[11]
  i950.softVegetation = !!i951[12]
  i950.activeColorSpace = i951[13]
  i950.desiredColorSpace = i951[14]
  i950.masterTextureLimit = i951[15]
  i950.maxQueuedFrames = i951[16]
  i950.particleRaycastBudget = i951[17]
  i950.pixelLightCount = i951[18]
  i950.realtimeReflectionProbes = !!i951[19]
  i950.shadowCascade2Split = i951[20]
  i950.shadowCascade4Split = new pc.Vec3( i951[21], i951[22], i951[23] )
  i950.streamingMipmapsActive = !!i951[24]
  i950.vSyncCount = i951[25]
  i950.asyncUploadBufferSize = i951[26]
  i950.asyncUploadTimeSlice = i951[27]
  i950.billboardsFaceCameraPosition = !!i951[28]
  i950.shadowNearPlaneOffset = i951[29]
  i950.streamingMipmapsMemoryBudget = i951[30]
  i950.maximumLODLevel = i951[31]
  i950.streamingMipmapsAddAllCameras = !!i951[32]
  i950.streamingMipmapsMaxLevelReduction = i951[33]
  i950.streamingMipmapsRenderersPerFrame = i951[34]
  i950.resolutionScalingFixedDPIFactor = i951[35]
  i950.streamingMipmapsMaxFileIORequests = i951[36]
  i950.currentQualityLevel = i951[37]
  return i950
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame"] = function (request, data, root) {
  var i960 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame' )
  var i961 = data
  i960.weight = i961[0]
  i960.vertices = i961[1]
  i960.normals = i961[2]
  i960.tangents = i961[3]
  return i960
}

Deserializers["UnityEngine.Events.ArgumentCache"] = function (request, data, root) {
  var i962 = root || request.c( 'UnityEngine.Events.ArgumentCache' )
  var i963 = data
  request.r(i963[0], i963[1], 0, i962, 'm_ObjectArgument')
  i962.m_ObjectArgumentAssemblyTypeName = i963[2]
  i962.m_IntArgument = i963[3]
  i962.m_FloatArgument = i963[4]
  i962.m_StringArgument = i963[5]
  i962.m_BoolArgument = !!i963[6]
  return i962
}

Deserializers.fields = {"Luna.Unity.DTO.UnityEngine.Assets.Mesh":{"name":0,"halfPrecision":1,"useSimplification":2,"useUInt32IndexFormat":3,"vertexCount":4,"aabb":5,"streams":6,"vertices":7,"subMeshes":8,"bindposes":9,"blendShapes":10},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh":{"triangles":0},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape":{"name":0,"frames":1},"Luna.Unity.DTO.UnityEngine.Assets.Material":{"name":0,"shader":1,"renderQueue":3,"enableInstancing":4,"floatParameters":5,"colorParameters":6,"vectorParameters":7,"textureParameters":8,"materialFlags":9},"Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag":{"name":0,"enabled":1},"Luna.Unity.DTO.UnityEngine.Textures.Texture2D":{"name":0,"width":1,"height":2,"mipmapCount":3,"anisoLevel":4,"filterMode":5,"hdr":6,"format":7,"wrapMode":8,"alphaIsTransparency":9,"alphaSource":10,"graphicsFormat":11,"sRGBTexture":12,"desiredColorSpace":13,"wrapU":14,"wrapV":15},"Luna.Unity.DTO.UnityEngine.Components.Transform":{"position":0,"scale":3,"rotation":6},"Luna.Unity.DTO.UnityEngine.Components.BoxCollider":{"center":0,"size":3,"enabled":6,"isTrigger":7,"material":8},"Luna.Unity.DTO.UnityEngine.Components.Animation":{"playAutomatically":0,"clip":1,"clips":3,"wrapMode":4,"enabled":5},"Luna.Unity.DTO.UnityEngine.Scene.GameObject":{"name":0,"tagId":1,"enabled":2,"isStatic":3,"layer":4},"Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer":{"sharedMesh":0,"bones":2,"updateWhenOffscreen":3,"localBounds":4,"rootBone":5,"blendShapesWeights":7,"enabled":8,"sharedMaterial":9,"sharedMaterials":11,"receiveShadows":12,"shadowCastingMode":13,"sortingLayerID":14,"sortingOrder":15,"lightmapIndex":16,"lightmapSceneIndex":17,"lightmapScaleOffset":18,"lightProbeUsage":22,"reflectionProbeUsage":23},"Luna.Unity.DTO.UnityEngine.Components.SkinnedMeshRenderer+BlendShapeWeight":{"weight":0},"Luna.Unity.DTO.UnityEngine.Components.RectTransform":{"pivot":0,"anchorMin":2,"anchorMax":4,"sizeDelta":6,"anchoredPosition3D":8,"rotation":11,"scale":15},"Luna.Unity.DTO.UnityEngine.Components.Canvas":{"planeDistance":0,"referencePixelsPerUnit":1,"isFallbackOverlay":2,"renderMode":3,"renderOrder":4,"sortingLayerName":5,"sortingOrder":6,"scaleFactor":7,"worldCamera":8,"overrideSorting":10,"pixelPerfect":11,"targetDisplay":12,"overridePixelPerfect":13,"enabled":14},"Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer":{"cullTransparentMesh":0},"Luna.Unity.DTO.UnityEngine.Components.MeshFilter":{"sharedMesh":0},"Luna.Unity.DTO.UnityEngine.Components.MeshRenderer":{"additionalVertexStreams":0,"enabled":2,"sharedMaterial":3,"sharedMaterials":5,"receiveShadows":6,"shadowCastingMode":7,"sortingLayerID":8,"sortingOrder":9,"lightmapIndex":10,"lightmapSceneIndex":11,"lightmapScaleOffset":12,"lightProbeUsage":16,"reflectionProbeUsage":17},"Luna.Unity.DTO.UnityEngine.Components.CapsuleCollider":{"center":0,"radius":3,"height":4,"direction":5,"enabled":6,"isTrigger":7,"material":8},"Luna.Unity.DTO.UnityEngine.Components.CanvasGroup":{"m_Alpha":0,"m_Interactable":1,"m_BlocksRaycasts":2,"m_IgnoreParentGroups":3,"enabled":4},"Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer":{"color":0,"sprite":4,"flipX":6,"flipY":7,"drawMode":8,"size":9,"tileMode":11,"adaptiveModeThreshold":12,"maskInteraction":13,"spriteSortPoint":14,"enabled":15,"sharedMaterial":16,"sharedMaterials":18,"receiveShadows":19,"shadowCastingMode":20,"sortingLayerID":21,"sortingOrder":22,"lightmapIndex":23,"lightmapSceneIndex":24,"lightmapScaleOffset":25,"lightProbeUsage":29,"reflectionProbeUsage":30},"Luna.Unity.DTO.UnityEngine.Scene.Scene":{"name":0,"index":1,"startup":2},"Luna.Unity.DTO.UnityEngine.Components.Camera":{"aspect":0,"orthographic":1,"orthographicSize":2,"backgroundColor":3,"nearClipPlane":7,"farClipPlane":8,"fieldOfView":9,"depth":10,"clearFlags":11,"cullingMask":12,"rect":13,"targetTexture":14,"usePhysicalProperties":16,"focalLength":17,"sensorSize":18,"lensShift":20,"gateFit":22,"commandBufferCount":23,"cameraType":24,"enabled":25},"Luna.Unity.DTO.UnityEngine.Components.Light":{"type":0,"color":1,"cullingMask":5,"intensity":6,"range":7,"spotAngle":8,"shadows":9,"shadowNormalBias":10,"shadowBias":11,"shadowStrength":12,"shadowResolution":13,"lightmapBakeType":14,"renderMode":15,"cookie":16,"cookieSize":18,"shadowNearPlane":19,"occlusionMaskChannel":20,"isBaked":21,"mixedLightingMode":22,"enabled":23},"Luna.Unity.DTO.UnityEngine.Components.LineRenderer":{"textureMode":0,"alignment":1,"widthCurve":2,"colorGradient":3,"positions":4,"positionCount":5,"widthMultiplier":6,"startWidth":7,"endWidth":8,"numCornerVertices":9,"numCapVertices":10,"useWorldSpace":11,"loop":12,"startColor":13,"endColor":17,"generateLightingData":21,"enabled":22,"sharedMaterial":23,"sharedMaterials":25,"receiveShadows":26,"shadowCastingMode":27,"sortingLayerID":28,"sortingOrder":29,"lightmapIndex":30,"lightmapSceneIndex":31,"lightmapScaleOffset":32,"lightProbeUsage":36,"reflectionProbeUsage":37},"Luna.Unity.DTO.UnityEngine.Components.AudioSource":{"clip":0,"outputAudioMixerGroup":2,"playOnAwake":4,"loop":5,"time":6,"volume":7,"pitch":8,"enabled":9},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings":{"ambientIntensity":0,"reflectionIntensity":1,"ambientMode":2,"ambientLight":3,"ambientSkyColor":7,"ambientGroundColor":11,"ambientEquatorColor":15,"fogColor":19,"fogEndDistance":23,"fogStartDistance":24,"fogDensity":25,"fog":26,"skybox":27,"fogMode":29,"lightmaps":30,"lightProbes":31,"lightmapsMode":32,"mixedBakeMode":33,"environmentLightingMode":34,"ambientProbe":35,"customReflection":36,"defaultReflection":38,"defaultReflectionMode":40,"defaultReflectionResolution":41,"sunLightObjectId":42,"pixelLightCount":43,"defaultReflectionHDR":44,"hasLightDataAsset":45,"hasManualGenerate":46},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap":{"lightmapColor":0,"lightmapDirection":2,"shadowMask":4},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes":{"bakedProbes":0,"positions":1,"hullRays":2,"tetrahedra":3,"neighbours":4,"matrices":5},"Luna.Unity.DTO.UnityEngine.Assets.Shader":{"ShaderCompilationErrors":0,"name":1,"guid":2,"shaderDefinedKeywords":3,"passes":4,"usePasses":5,"defaultParameterValues":6,"unityFallbackShader":7,"readDepth":9,"hasDepthOnlyPass":10,"isCreatedByShaderGraph":11,"disableBatching":12,"compiled":13},"Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError":{"shaderName":0,"errorMessage":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass":{"id":0,"subShaderIndex":1,"name":2,"passType":3,"grabPassTextureName":4,"usePass":5,"zTest":6,"zWrite":7,"culling":8,"blending":9,"alphaBlending":10,"colorWriteMask":11,"offsetUnits":12,"offsetFactor":13,"stencilRef":14,"stencilReadMask":15,"stencilWriteMask":16,"stencilOp":17,"stencilOpFront":18,"stencilOpBack":19,"tags":20,"passDefinedKeywords":21,"passDefinedKeywordGroups":22,"variants":23,"excludedVariants":24,"hasDepthReader":25},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value":{"val":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending":{"src":0,"dst":1,"op":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp":{"pass":0,"fail":1,"zFail":2,"comp":3},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup":{"keywords":0,"hasDiscard":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant":{"passId":0,"subShaderIndex":1,"keywords":2,"vertexProgram":3,"fragmentProgram":4,"exportedForWebGl2":5,"readDepth":6},"Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass":{"shader":0,"pass":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue":{"name":0,"type":1,"value":2,"textureValue":6,"shaderPropertyFlag":7},"Luna.Unity.DTO.UnityEngine.Textures.Sprite":{"name":0,"texture":1,"aabb":3,"vertices":4,"triangles":5,"textureRect":6,"packedRect":10,"border":14,"transparency":18,"bounds":19,"pixelsPerUnit":20,"textureWidth":21,"textureHeight":22,"nativeSize":23,"pivot":25,"textureRectOffset":27},"Luna.Unity.DTO.UnityEngine.Assets.AudioClip":{"name":0},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip":{"name":0,"wrapMode":1,"isLooping":2,"length":3,"curves":4,"events":5,"halfPrecision":6,"_frameRate":7,"localBounds":8,"hasMuscleCurves":9,"clipMuscleConstant":10,"clipBindingConstant":11},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve":{"path":0,"hash":1,"componentType":2,"property":3,"keys":4,"objectReferenceKeys":5},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey":{"time":0,"value":1},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent":{"functionName":0,"floatParameter":1,"intParameter":2,"stringParameter":3,"objectReferenceParameter":4,"time":6},"Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds":{"center":0,"extends":3},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant":{"genericBindings":0,"pptrCurveMapping":1},"Luna.Unity.DTO.UnityEngine.Assets.Font":{"name":0,"ascent":1,"originalLineHeight":2,"fontSize":3,"characterInfo":4,"texture":5,"originalFontSize":7},"Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo":{"index":0,"advance":1,"bearing":2,"glyphWidth":3,"glyphHeight":4,"minX":5,"maxX":6,"minY":7,"maxY":8,"uvBottomLeftX":9,"uvBottomLeftY":10,"uvBottomRightX":11,"uvBottomRightY":12,"uvTopLeftX":13,"uvTopLeftY":14,"uvTopRightX":15,"uvTopRightY":16},"Luna.Unity.DTO.UnityEngine.Assets.Resources":{"files":0,"componentToPrefabIds":1},"Luna.Unity.DTO.UnityEngine.Assets.Resources+File":{"path":0,"unityObject":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings":{"scriptsExecutionOrder":0,"sortingLayers":1,"cullingLayers":2,"timeSettings":3,"physicsSettings":4,"physics2DSettings":5,"qualitySettings":6,"enableRealtimeShadows":7,"enableAutoInstancing":8,"enableStaticBatching":9,"enableDynamicBatching":10,"usePreservativeDynamicBatching":11,"lightmapEncodingQuality":12,"desiredColorSpace":13,"allTags":14},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer":{"id":0,"name":1,"value":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer":{"id":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings":{"fixedDeltaTime":0,"maximumDeltaTime":1,"timeScale":2,"maximumParticleTimestep":3},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings":{"gravity":0,"defaultSolverIterations":3,"bounceThreshold":4,"autoSyncTransforms":5,"autoSimulation":6,"collisionMatrix":7},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings":{"material":0,"gravity":2,"positionIterations":4,"velocityIterations":5,"velocityThreshold":6,"maxLinearCorrection":7,"maxAngularCorrection":8,"maxTranslationSpeed":9,"maxRotationSpeed":10,"baumgarteScale":11,"baumgarteTOIScale":12,"timeToSleep":13,"linearSleepTolerance":14,"angularSleepTolerance":15,"defaultContactOffset":16,"autoSimulation":17,"queriesHitTriggers":18,"queriesStartInColliders":19,"callbacksOnDisable":20,"reuseCollisionCallbacks":21,"autoSyncTransforms":22,"collisionMatrix":23},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.QualitySettings":{"qualityLevels":0,"names":1,"shadows":2,"anisotropicFiltering":3,"antiAliasing":4,"lodBias":5,"shadowCascades":6,"shadowDistance":7,"shadowmaskMode":8,"shadowProjection":9,"shadowResolution":10,"softParticles":11,"softVegetation":12,"activeColorSpace":13,"desiredColorSpace":14,"masterTextureLimit":15,"maxQueuedFrames":16,"particleRaycastBudget":17,"pixelLightCount":18,"realtimeReflectionProbes":19,"shadowCascade2Split":20,"shadowCascade4Split":21,"streamingMipmapsActive":24,"vSyncCount":25,"asyncUploadBufferSize":26,"asyncUploadTimeSlice":27,"billboardsFaceCameraPosition":28,"shadowNearPlaneOffset":29,"streamingMipmapsMemoryBudget":30,"maximumLODLevel":31,"streamingMipmapsAddAllCameras":32,"streamingMipmapsMaxLevelReduction":33,"streamingMipmapsRenderersPerFrame":34,"resolutionScalingFixedDPIFactor":35,"streamingMipmapsMaxFileIORequests":36,"currentQualityLevel":37},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame":{"weight":0,"vertices":1,"normals":2,"tangents":3}}

Deserializers.requiredComponents = {"69":[70],"71":[70],"72":[70],"73":[70],"74":[70],"75":[70],"76":[10],"77":[33],"78":[79],"80":[79],"81":[79],"82":[79],"83":[79],"84":[79],"85":[79],"86":[87],"88":[87],"89":[87],"90":[87],"91":[87],"92":[87],"93":[87],"94":[87],"95":[87],"96":[87],"97":[87],"98":[87],"99":[87],"100":[33],"101":[25],"102":[103],"104":[103],"16":[15],"61":[15],"105":[15],"106":[15],"30":[16],"14":[19,15],"51":[15],"18":[16],"107":[15],"108":[15],"109":[15],"110":[15],"111":[15],"112":[15],"113":[15],"114":[15],"115":[15],"116":[19,15],"117":[15],"118":[15],"119":[15],"120":[15],"13":[19,15],"121":[15],"122":[64],"123":[64],"65":[64],"124":[64],"125":[33],"126":[33]}

Deserializers.types = ["UnityEngine.Shader","UnityEngine.Texture2D","UnityEngine.Transform","UnityEngine.BoxCollider","UnityEngine.MonoBehaviour","Project.Gameplay.Units.PlayerView","Project.Gameplay.Units.PowerLabel","UnityEngine.GameObject","UnityEngine.Animation","UnityEngine.AnimationClip","UnityEngine.SkinnedMeshRenderer","UnityEngine.Mesh","UnityEngine.Material","UnityEngine.UI.Text","UnityEngine.UI.Image","UnityEngine.RectTransform","UnityEngine.Canvas","UnityEngine.EventSystems.UIBehaviour","UnityEngine.UI.CanvasScaler","UnityEngine.CanvasRenderer","UnityEngine.Sprite","UnityEngine.Font","Project.Gameplay.Vfx.FootstepEmitter","Project.Gameplay.Units.ChestView","UnityEngine.MeshFilter","UnityEngine.MeshRenderer","UnityEngine.CapsuleCollider","Project.Gameplay.Units.EnemyView","Project.Gameplay.Vfx.FloatingNumber","UnityEngine.CanvasGroup","UnityEngine.UI.GraphicRaycaster","UnityEngine.SpriteRenderer","Project.Gameplay.Vfx.SpriteSequencePlayer","UnityEngine.Camera","UnityEngine.AudioListener","Project.Gameplay.CameraFx.ScreenShake","UnityEngine.Light","Project.Gameplay.UnitSpawner","Project.Configs.UnitsBank","Project.Gameplay.Flow.BattleFlow","Project.Gameplay.Targeting.TapInput","Project.Gameplay.Targeting.TargetIndicator","Project.Gameplay.Vfx.VfxService","Project.Gameplay.Vfx.VfxBank","UnityEngine.LineRenderer","Project.Audio.AudioService","Project.Core.GameRoot","Project.Audio.AudioBank","UnityEngine.AudioSource","Project.Integration.MraidBridge","Project.Integration.AnalyticsService","UnityEngine.UI.AspectRatioFitter","Project.UI.Hud.HudView","Project.UI.Hud.HudPresenter","UnityEngine.UI.Button","Project.UI.Common.MuteToggle","Project.UI.Common.TutorialNudge","Project.UI.Common.MobCounter","Project.UI.Common.TouchRipple","Project.UI.EndCard.EndCardView","Project.UI.EndCard.EndCardPresenter","Project.UI.Common.TweenButton","Project.UI.Common.UiSoundButton","Project.UI.Common.TapToStartSplash","UnityEngine.EventSystems.EventSystem","UnityEngine.EventSystems.StandaloneInputModule","Project.Configs.LevelConfig","Project.Configs.BalanceConfig","UnityEngine.AudioClip","UnityEngine.AudioLowPassFilter","UnityEngine.AudioBehaviour","UnityEngine.AudioHighPassFilter","UnityEngine.AudioReverbFilter","UnityEngine.AudioDistortionFilter","UnityEngine.AudioEchoFilter","UnityEngine.AudioChorusFilter","UnityEngine.Cloth","UnityEngine.FlareLayer","UnityEngine.ConstantForce","UnityEngine.Rigidbody","UnityEngine.Joint","UnityEngine.HingeJoint","UnityEngine.SpringJoint","UnityEngine.FixedJoint","UnityEngine.CharacterJoint","UnityEngine.ConfigurableJoint","UnityEngine.CompositeCollider2D","UnityEngine.Rigidbody2D","UnityEngine.Joint2D","UnityEngine.AnchoredJoint2D","UnityEngine.SpringJoint2D","UnityEngine.DistanceJoint2D","UnityEngine.FrictionJoint2D","UnityEngine.HingeJoint2D","UnityEngine.RelativeJoint2D","UnityEngine.SliderJoint2D","UnityEngine.TargetJoint2D","UnityEngine.FixedJoint2D","UnityEngine.WheelJoint2D","UnityEngine.ConstantForce2D","UnityEngine.StreamingController","UnityEngine.TextMesh","UnityEngine.Tilemaps.TilemapRenderer","UnityEngine.Tilemaps.Tilemap","UnityEngine.Tilemaps.TilemapCollider2D","UnityEngine.UI.Dropdown","UnityEngine.UI.Graphic","UnityEngine.UI.ContentSizeFitter","UnityEngine.UI.GridLayoutGroup","UnityEngine.UI.HorizontalLayoutGroup","UnityEngine.UI.HorizontalOrVerticalLayoutGroup","UnityEngine.UI.LayoutElement","UnityEngine.UI.LayoutGroup","UnityEngine.UI.VerticalLayoutGroup","UnityEngine.UI.Mask","UnityEngine.UI.MaskableGraphic","UnityEngine.UI.RawImage","UnityEngine.UI.RectMask2D","UnityEngine.UI.Scrollbar","UnityEngine.UI.ScrollRect","UnityEngine.UI.Slider","UnityEngine.UI.Toggle","UnityEngine.EventSystems.BaseInputModule","UnityEngine.EventSystems.PointerInputModule","UnityEngine.EventSystems.TouchInputModule","UnityEngine.EventSystems.Physics2DRaycaster","UnityEngine.EventSystems.PhysicsRaycaster"]

Deserializers.unityVersion = "2022.3.62f2";

Deserializers.productName = "Playable ads";

Deserializers.lunaInitializationTime = "05/04/2026 21:09:13";

Deserializers.lunaDaysRunning = "2.0";

Deserializers.lunaVersion = "7.2.0";

Deserializers.lunaSHA = "ea08d29afe2968efcb8d91d5624f033c6485cc68";

Deserializers.creativeName = "playable_ads";

Deserializers.lunaAppID = "39366";

Deserializers.projectId = "1c7acbfa9d567284385ee3aa77581220";

Deserializers.packagesInfo = "com.unity.ugui: 1.0.0";

Deserializers.externalJsLibraries = "";

Deserializers.androidLink = ( typeof window !== "undefined")&&window.$environment.packageConfig.androidLink?window.$environment.packageConfig.androidLink:'Empty';

Deserializers.iosLink = ( typeof window !== "undefined")&&window.$environment.packageConfig.iosLink?window.$environment.packageConfig.iosLink:'Empty';

Deserializers.base64Enabled = "False";

Deserializers.minifyEnabled = "True";

Deserializers.isForceUncompressed = "False";

Deserializers.isAntiAliasingEnabled = "False";

Deserializers.isRuntimeAnalysisEnabledForCode = "True";

Deserializers.runtimeAnalysisExcludedClassesCount = "1558";

Deserializers.runtimeAnalysisExcludedMethodsCount = "3466";

Deserializers.runtimeAnalysisExcludedModules = "physics2d, particle-system, reflection";

Deserializers.isRuntimeAnalysisEnabledForShaders = "False";

Deserializers.isRealtimeShadowsEnabled = "False";

Deserializers.isLunaCompilerV2Used = "True";

Deserializers.companyName = "DefaultCompany";

Deserializers.buildPlatform = "WebGL";

Deserializers.applicationIdentifier = "com.DefaultCompany.Playable-ads";

Deserializers.disableAntiAliasing = true;

Deserializers.graphicsConstraint = 24;

Deserializers.linearColorSpace = false;

Deserializers.buildID = "3794a54c-b93c-4bab-8dd2-b4f0604b1dc8";

Deserializers.runtimeInitializeOnLoadInfos = [[["UnityEngine","Experimental","Rendering","ScriptableRuntimeReflectionSystemSettings","ScriptingDirtyReflectionSystemInstance"]],[],[],[],[]];

Deserializers.typeNameToIdMap = function(){ var i = 0; return Deserializers.types.reduce( function( res, item ) { res[ item ] = i++; return res; }, {} ) }()

