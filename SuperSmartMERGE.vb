' SuperSmartMERGE v1.1
' A smarter Merge transition for vMix.
' by Aden Beckitt
' https://github.com/patriot1889/
' 
' Original SmartMERGE script by STREAMING ALCHEMY
' https://github.com/StreamingAlchemy/StreamingAlchemy

' *************************************************************
' *****  Adjust LAYERS to perform Clean MERGE Transition  *****
' *************************************************************

dim mergeDuration as String = "1000"    ' Enter duration for merge transition

'The KEYS of PROGRAM and PREVIEW
dim PROGInput as String = ""            ' The KEY (AS String) of the INPUT NOW in PROGRAM
dim PREVInput as String = ""            ' The KEY (AS String) of the INPUT NOW in PREVIEW

'INPUT NUMBERS of PROGRAM and PREVIEW
dim PROGInputNumber as String = ""      ' The NUMBER (AS String) of the INPUT NOW in PROGRAM
dim PREVInputNumber as String = ""      ' The NUMBER (AS String) of the INPUT NOW in PREVIEW

dim FoundLayerPREV as String = ""           ' The INDEX of the Layer we Found
dim LoopLayerPREV as String = ""           ' The INDEX of the Layer we Found
dim FoundLayerPROG as String = ""           ' The INDEX of the Layer we Found
dim LoopLayerPROG as String = ""           ' The INDEX of the Layer we Found
dim FoundLayer as String = ""           ' The INDEX of the Layer we Found

'LAYER Counts
dim TotalPROGLayers as Integer = 0      ' Total Number of layers in PROGRAM
dim TotalPREVLayers as Integer = 0      ' Total Number of layers in PREVIEW
dim TotalMatchingLayers as Integer = 0      ' Total Number of matching layers

'XML Components
dim PROGInputNodeList As XMLNodeList    ' NodeList of PROGRAM layers
dim PROGInputNode As XMLNode            ' The XMLNode of PROGRAM INPUT
dim PREVInputNodeList As XMLNodeList    ' NodeList of PREVIEW layers
dim PREVInputNode As XMLNode            ' The XMLNode of PREVIEW INPUT

dim FoundLayerPREVNode As XMLNode           ' The XMLNode to test for Specific Layer with PROG or PREV input
dim FoundLayerPROGNode As XMLNode           ' The XMLNode to test for Specific Layer with PROG or PREV input
dim FoundLayerNode As XMLNode           ' The XMLNode to test for Specific Layer with PROG or PREV input

' *****  PROCESSING STARTS HERE  *****

' Load the vMix XML Model
dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)


' *****  GET DETAILS FOR INPUTS IN PROGRAM AND PREVIEW *****

'Get the XMLNode for the Input in PROGRAM:
PROGInputNumber = VmixXML.SelectSingleNode("/vmix/active").InnerText
PROGInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@number=" & PROGInputNumber & "]")
PROGInput = PROGInputNode.Attributes.GetNamedItem("key").Value

'Get the XMLNode for the Input in PREVIEW:
PREVInputNumber = VmixXML.SelectSingleNode("/vmix/preview").InnerText
PREVInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@number=" & PREVInputNumber & "]")
PREVInput = PREVInputNode.Attributes.GetNamedItem("key").Value 

'The XMLNodeList of LAYERS in PROGRAM:
PROGInputNodeList = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]").SelectNodes("overlay") 

'The XMLNodeList of LAYERS in PREVIEW:
PREVInputNodeList = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PREVInput & """]").SelectNodes("overlay") 

'Get the count of layers we're working with:
TotalPROGLayers = PROGInputNodeList.Count
TotalPREVLayers = PREVInputNodeList.Count

If TotalPROGLayers>0 AND TotalPREVLayers>0 Then 'PREVIEW and PROGRAM both have layers
    For Each layer As XmlNode in PROGInputNodeList
        LoopLayerPROG = layer.Attributes.GetNamedItem("key").Value
        FoundLayerPREVNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PREVInput & """]/overlay[@key=""" & LoopLayerPROG & """]") 
        If NOT FoundLayerPREVNode IS NOTHING Then
            TotalMatchingLayers = TotalMatchingLayers + 1
            FoundLayerPREV = FoundLayerPREVNode.Attributes.GetNamedItem("index").Value
        End If
    Next layer
    
    For Each layer As XmlNode in PREVInputNodeList
        LoopLayerPREV = layer.Attributes.GetNamedItem("key").Value
        FoundLayerPROGNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & LoopLayerPREV & """]") 
        If NOT FoundLayerPROGNode IS NOTHING Then
            FoundLayerPROG = FoundLayerPROGNode.Attributes.GetNamedItem("index").Value
        End If
    Next layer
    
    If NOT FoundLayerPREV IS NOTHING AND TotalMatchingLayers>1 Then
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:=(CInt(FoundLayerPREV) + 1) & ",10")    'PREV:Move FoundLayerPREV to LAYER 10
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:=(CInt(FoundLayerPROG) + 1) & ",10")    'PROG:Move FoundLayerPROG to LAYER 10
        Sleep(100)
        API.Function("Merge", Duration:=mergeDuration)
        Sleep(1000)
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:="10," & (CInt(FoundLayerPREV) + 1))     'PREV:Move Layer 10 back to FoundLayerPREV
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:="10," & (CInt(FoundLayerPROG) + 1))     'PROG:Move Layer 10 back to FoundLayerPROG
    Else
        API.Function("Merge", Duration:=mergeDuration)
        If TotalMatchingLayers<=1 Then
            Console.WriteLine("TotalMatchingLayers not greater than ONE. (Likely just background layer) Doing simple MERGE")
        End If
    End If
    
Else If TotalPROGLayers<=0 AND TotalPREVLayers>0 Then  'LAYERS are in PREVIEW with just an INPUT in PROGRAM
        'Check if PROGRAM's Input is in one of the PREVIEW's Layers and Assign it to FoundLAYER
    FoundLAYERNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PREVInput & """]/overlay[@key=""" & PROGInput & """]")
   
    If NOT FoundLAYERNode IS NOTHING Then   ' If there is a LAYER with the INPUT in PROGRAM...
        FoundLAYER = FoundLAYERNode.Attributes.GetNamedItem("index").Value    ' ...Assign the INDEX of the LAYER
       
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:=(CInt(FoundLAYER) + 1) & ",10")    'PREV:Move FoundLAYER to LAYER 10
        Sleep(100)
        API.Function("Merge", Duration:=mergeDuration)
        Sleep(1000)
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:="10," & (CInt(FoundLAYER) + 1))     'PROG:Move Layer 10 back to FoundLAYER
    Else
        API.Function("Merge", Duration:=mergeDuration)
    End If
    
Else If TotalPROGLayers>0 AND TotalPREVLayers<=0 Then  'LAYERS are in PROGRAM with an just INPUT in PREVIEW
    
    'Check if PROGRAM's Input is in one of the PREVIEW's Layers and Assign it to FoundLAYER
    FoundLAYERNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & PREVInput & """]")
       
    If NOT FoundLAYERNode IS NOTHING Then   ' If there is a LAYER with the INPUT in PREVIEW...
        FoundLAYER = FoundLAYERNode.Attributes.GetNamedItem("index").Value    ' ...Assign the INDEX of the LAYER
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:=(CInt(FoundLAYER) + 1) & ",10")    'PROG:Move FoundLAYER to LAYER 10
        Sleep(100)
        API.Function("Merge", Duration:=mergeDuration)
        Sleep(1000)
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:="10," & (CInt(FoundLAYER) + 1)) 'PROG:Move Layer 10 back to FoundLAYER
    Else
        
        API.Function("Merge", Duration:=mergeDuration)  
    End If
    
Else 'Neither PROGRAM or PREVIEW have layers
    API.Function("Merge", Duration:=mergeDuration)   'Just do the MERGE without any adjustments
End If
