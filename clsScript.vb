﻿Public Class clsScript
    Implements IDisposable
    Public actions As List(Of clsAction)
    Public stateActions As List(Of clsStatelessAction)
    Public lastAction As clsAction
    Public state As scriptState
    Public totalWait As Integer
    Private controllers As Generic.Dictionary(Of String, clsController)
    Private capture As clsSnapshot

    Enum scriptState
        ready
        running
        paused
        finished
        scriptError
    End Enum

    Public Function createHoldSA(parent As clsAction, timecode As Integer, buttonMask As Integer, LT As Integer, RT As Integer, LS As Point, RS As Point) As List(Of clsSimpleAction)
        Dim tmp As New List(Of clsSimpleAction)
        Dim sa As clsSimpleAction = Nothing
        If LT >= 0 Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLT, LT, parent)
            tmp.Add(sa)
        End If
        If RT >= 0 Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRT, RT, parent)
            tmp.Add(sa)
        End If
        If LS.X > -128 And LS.Y > -128 Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLSX, LS.X, parent)
            tmp.Add(sa)
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLSY, LS.Y, parent)
            tmp.Add(sa)
        End If
        If RS.X > -128 And RS.Y > -128 Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRSX, RS.X, parent)
            tmp.Add(sa)
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRSY, RS.Y, parent)
            tmp.Add(sa)
        End If
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnA)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnB)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnX)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnY)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnLB)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnRB)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnL3)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnR3)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnUp)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnDown)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnLeft)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnRight)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnBack)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnStart)
        addButtonSA(tmp, parent, timecode, buttonMask, 1, clsController.XBButtons.btnGuide)
        Return tmp
    End Function

    Sub addButtonSA(list As List(Of clsSimpleAction), parent As clsAction, timecode As Integer, buttonmask As Integer, value As Integer, button As Integer)
        If buttonmask And button Then
            Dim sa As New clsSimpleAction(parent.controllerNumber, timecode, button, value, parent)
            list.Add(sa)
        End If
    End Sub

    Public Function createReleaseSA(parent As clsAction, timecode As Integer, buttonMask As Integer, LTDefined As Boolean, RTDefined As Boolean, LSDefined As Boolean, RSdefined As Boolean) As List(Of clsSimpleAction)
        Dim tmp As New List(Of clsSimpleAction)
        Dim sa As clsSimpleAction = Nothing
        If LTDefined Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLT, 0, parent)
            tmp.Add(sa)
        End If
        If RTDefined Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRT, 0, parent)
            tmp.Add(sa)
        End If
        If LSDefined Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLSX, 0, parent)
            tmp.Add(sa)
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnLSY, 0, parent)
            tmp.Add(sa)
        End If
        If RSdefined Then
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRSX, 0, parent)
            tmp.Add(sa)
            sa = New clsSimpleAction(parent.controllerNumber, timecode, clsSimpleAction.saButtons.btnRSY, 0, parent)
            tmp.Add(sa)
        End If
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnA)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnB)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnX)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnY)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnLB)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnRB)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnL3)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnR3)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnUp)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnDown)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnLeft)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnRight)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnBack)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnStart)
        addButtonSA(tmp, parent, timecode, buttonMask, 0, clsController.XBButtons.btnGuide)
        Return tmp
    End Function

    Public Function createCapture() As clsSnapshot
        Dim curDevPath As String = GetSetting(Application.ProductName, "Settings", "CapDevPath", vbNullString)
        If curDevPath = vbNullString Then
            MsgBox("Capture Device not set.")
            Return Nothing
        End If
        Try
            Return New clsSnapshot(curDevPath)
        Catch ex As Exception
            MsgBox("Capture Device error:" & vbCrLf & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Sub New(_actions As List(Of clsAction), controllerIPS As Dictionary(Of Byte, String))
        actions = _actions
        Dim i As Integer = 0
        Dim timecode As Integer = 0
        Dim simpleActions As New List(Of clsSimpleAction)
        controllers = New Generic.Dictionary(Of String, clsController)
        For Each action As clsAction In actions
            Select Case action.getActType
                Case ActionType.actLoop
                    CType(action, clsActionLoop).repeatLeft = CType(action, clsActionLoop).repeat
                Case ActionType.actInputVideo
                    If capture Is Nothing Then capture = createCapture()
                    If capture Is Nothing Then
                        state = scriptState.scriptError
                        Exit Sub
                    End If
                    CType(action, clsActionInputVideo).capture = capture
            End Select
            If action.controllerNumber AndAlso Not controllers.ContainsKey(action.controllerNumber) Then
                If Not controllerIPS.ContainsKey(action.controllerNumber) Then Stop
                Dim ip As String = controllerIPS(action.controllerNumber)
                If System.Text.RegularExpressions.Regex.IsMatch(ip, "^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") Then
                    controllers.Add(action.controllerNumber, New clsBBBController(ip))
                ElseIf System.Text.RegularExpressions.Regex.IsMatch(ip, "^COM[1-9][0-9]?$") Then
                    controllers.Add(action.controllerNumber, New clsPS2Controller(ip))
                ElseIf ip.StartsWith("CM") Then
                    controllers.Add(action.controllerNumber, New clsCMHIDController(ip))
                Else
                    Stop
                End If
            End If
        Next
        For Each number As Byte In controllers.Keys
            Dim finalAction As New clsActionRelease(number, &HFFFF, 0, 0, New Point(0, 0), New Point(0, 0), Nothing)
            finalAction.index = actions.Count
            actions.Add(finalAction)
        Next
        While i < actions.Count
            'Debug.Print(i & "," & timecode & "," & actions(i).toString)
            Select Case actions(i).getActType
                Case ActionType.actGroup
                    i = i + 1
                Case ActionType.actHold
                    Dim actHold As clsActionHold = actions(i)
                    simpleActions.AddRange(createHoldSA(actions(i), timecode, actHold.buttonMask, actHold.LT, actHold.RT, actHold.LS, actHold.RS))
                    i = i + 1
                Case ActionType.actRelease
                    Dim actRelease As clsActionRelease = actions(i)
                    simpleActions.AddRange(createReleaseSA(actions(i), timecode, actRelease.buttonMask, actRelease.LTDefined, actRelease.RTDefined, actRelease.LSDefined, actRelease.RSDefined))
                    i = i + 1
                Case ActionType.actPress
                    Dim actPress As clsActionPress = actions(i)
                    For j = 1 To actPress.repeat
                        simpleActions.AddRange(createHoldSA(actions(i), timecode, actPress.buttonMask, actPress.LT, actPress.RT, actPress.LS, actPress.RS))
                        simpleActions.AddRange(createReleaseSA(actions(i), timecode + actPress.holdTime, actPress.buttonMask, actPress.LTDefined, actPress.RTDefined, actPress.LSDefined, actPress.RSDefined))
                        timecode = timecode + actPress.waitTime
                    Next
                    i = i + 1
                Case ActionType.actLoop
                    Dim actLoop As clsActionLoop = actions(i)
                    If actLoop.repeatLeft <= 1 Then
                        actLoop.repeatLeft = actLoop.repeat
                        i = i + 1
                    Else
                        actLoop.repeatLeft -= 1
                        Dim tgt As clsAction = CType(actions(i), clsActionLoop).target
                        i = actions.IndexOf(tgt)
                    End If
                Case ActionType.actWait
                    Dim actWait As clsActionWait = actions(i)
                    simpleActions.Add(New clsSimpleAction(timecode, actions(i)))
                    timecode = timecode + actWait.delay
                    i = i + 1
                Case ActionType.actInputVideo
                    simpleActions.Add(New clsSimpleAction(timecode, actions(i)))
                    timecode = timecode + 1
                    i = i + 1
            End Select
        End While
        simpleActions.Sort()

        stateActions = New List(Of clsStatelessAction)
        Dim curOffset As Integer = -1
        Dim curController As Byte = 0
        Dim lastAction As clsSimpleAction = Nothing

        For Each action In simpleActions
            If action.timeoffset <> curOffset OrElse action.controllerNumber <> curController Then
                If curOffset <> -1 Then
                    If lastAction.wait Then
                        stateActions.Add(New clsStatelessAction(curOffset, lastAction.parent))
                    Else
                        stateActions.Add(New clsStatelessAction(controllers(curController), controllers(curController).getReport(), curOffset, lastAction.parent))
                    End If
                End If
                curController = action.controllerNumber
                curOffset = action.timeoffset
                lastAction = action
            End If
            If Not action.wait AndAlso Not action.input Then
                If action.button <= clsSimpleAction.saButtons.btnR3 Then
                    If action.value Then controllers(curController).pressButtons(action.button, False) Else controllers(curController).releaseButtons(action.button, False)
                End If
                If action.button = clsSimpleAction.saButtons.btnLT Then controllers(curController).setLT(action.value, False)
                If action.button = clsSimpleAction.saButtons.btnRT Then controllers(curController).setRT(action.value, False)
                If action.button = clsSimpleAction.saButtons.btnLSX Then controllers(curController).setJoyLSX(action.value, False)
                If action.button = clsSimpleAction.saButtons.btnLSY Then controllers(curController).setJoyLSY(action.value, False)
                If action.button = clsSimpleAction.saButtons.btnRSX Then controllers(curController).setJoyRSX(action.value, False)
                If action.button = clsSimpleAction.saButtons.btnRSY Then controllers(curController).setJoyRSY(action.value, False)
            End If
        Next
        stateActions.Add(New clsStatelessAction(controllers(curController), controllers(curController).getReport(), curOffset, lastAction.parent))
    End Sub

    Private startTime As Date
    Private pauseTime As Date
    Private pauseFlag As Boolean
    Private stopFlag As Boolean
    Private runThread As Threading.Thread

    Private Sub waitforcontinue()
        Me.state = scriptState.paused
        While pauseFlag
            System.Threading.Thread.Sleep(50)
            If stopFlag Then
                Me.state = scriptState.finished
                Exit Sub
            End If
        End While
        Me.state = scriptState.running
    End Sub

    Private Sub runScript()
        Me.state = scriptState.running
        Dim i As Integer = 0
        Dim curAction As clsStatelessAction = stateActions(i)
        Dim nextTime As Integer = curAction.timeoffset
        Dim curTime As Integer
        Dim waitTime As Integer
        Dim inputAction As clsActionInput = Nothing
        For Each controller As clsController In controllers.Values
            controller.resetController()
        Next
        Do
            If pauseFlag Then waitforcontinue()
            If stopFlag Then Exit Do
            If Not inputAction Is Nothing Then
                If Now >= inputAction.nextTest AndAlso inputAction.test Then
                    Debug.Print("Adding " & (Now - inputAction.startTime).TotalMilliseconds & " to clock.")
                    startTime = startTime.Add(Now - inputAction.startTime)
                    i = i + 1
                    If i > stateActions.Count - 1 Then Exit Do
                    lastAction = curAction.parent
                    curAction = stateActions(i)
                    nextTime = curAction.timeoffset
                    inputAction = Nothing
                Else
                    waitTime = (inputAction.nextTest - Now).TotalMilliseconds
                    If waitTime > 300 Then waitTime = 250
                    If waitTime > 0 Then System.Threading.Thread.Sleep(waitTime)
                End If
            Else
                curTime = (Now - startTime).TotalMilliseconds
                waitTime = nextTime - curTime
                If waitTime <= 0 Then
                    Debug.Print(curAction.parent.toString)
                    If curAction.input Then
                        inputAction = CType(curAction.parent, clsActionInput)
                        inputAction.start()
                    ElseIf curAction.wait Then
                        i = i + 1
                        If i > stateActions.Count - 1 Then Exit Do
                        lastAction = curAction.parent
                        curAction = stateActions(i)
                        nextTime = curAction.timeoffset
                    Else
                        curAction.controller.sendReport(curAction.report)
                        i = i + 1
                        If i > stateActions.Count - 1 Then Exit Do
                        lastAction = curAction.parent
                        curAction = stateActions(i)
                        nextTime = curAction.timeoffset
                    End If
                Else
                    totalWait = waitTime
                    If waitTime > 300 Then waitTime = 250
                    System.Threading.Thread.Sleep(waitTime)
                End If
            End If
        Loop
        If stopFlag Then
            For Each controller As clsController In controllers.Values
                controller.resetController()
            Next
        End If
        For Each controller As clsController In controllers.Values
            controller.dispose()
        Next
        Me.state = scriptState.finished
        Me.dispose()
    End Sub

    Public Sub startScript()
        startTime = Now
        stopFlag = False
        pauseFlag = False
        Dim ts As New System.Threading.ThreadStart(AddressOf Me.runScript)
        runThread = New System.Threading.Thread(ts)
        runThread.Start()
    End Sub

    Public Sub pauseScript()
        pauseFlag = True
        pauseTime = Now
        Do
            System.Threading.Thread.Sleep(100)
        Loop Until state = scriptState.paused Or state = scriptState.finished
    End Sub

    Public Sub continueScript()
        startTime = startTime.Add(Now - pauseTime)
        pauseFlag = False
    End Sub

    Public Sub stopScript()
        stopFlag = True
        runThread.Join()
        Me.dispose()
    End Sub

    Public Sub dispose() Implements IDisposable.Dispose
        Finalize()
    End Sub

    Protected Overrides Sub Finalize()
        If Not capture Is Nothing Then
            capture.Dispose()
            capture = Nothing
        End If
        MyBase.Finalize()
    End Sub
End Class
