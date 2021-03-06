﻿Imports System.Text.RegularExpressions
Imports System.Xml
Public Class frmEdit
    Private valLS As Point
    Private valRS As Point
    Private valLT As Integer
    Private valRT As Integer
    Private loopTarget As clsAction
    Private groupTarget As clsActionGroup
    Private filename As String

    Private Const mainGroup As String = "[Main]"
    Private activeScript As clsScript
    Private groups As New Dictionary(Of String, clsActionGroup)
    Private activeGroup As clsActionGroup = Nothing

    Private Sub frmEdit_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        saveSettings()
    End Sub

    Private Sub saveSettings()
        SaveSetting(Application.ProductName, "Settings", "Controller1", txtController1.Text)
        SaveSetting(Application.ProductName, "Settings", "Controller2", txtController2.Text)
        SaveSetting(Application.ProductName, "Settings", "Controller3", txtController3.Text)
        SaveSetting(Application.ProductName, "Settings", "Controller4", txtController4.Text)
        SaveSetting(Application.ProductName, "Settings", "Precompile", IIf(cbPrecompile.Checked, "True", "False"))
        SaveSetting(Application.ProductName, "Settings", "Trace", IIf(cbTrace.Checked, "True", "False"))
        SaveSetting(Application.ProductName, "Settings", "SyncWait", IIf(cbSyncWait.Checked, "True", "False"))
        SaveSetting(Application.ProductName, "Settings", "Sync", IIf(cbSync.Checked, "True", "False"))
        SaveSetting(Application.ProductName, "Settings", "SyncIP", txtSync.Text)
    End Sub

    Private Sub frmEdit_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim tTip As New ToolTip
        tTip.AutomaticDelay = 1000
        tTip.SetToolTip(btnAdd, "Add Action")
        tTip.SetToolTip(btnApply, "Apply Changes")
        tTip.SetToolTip(btnMoveDown, "Move Down")
        tTip.SetToolTip(btnMoveUp, "Move Up")
        tTip.SetToolTip(btnDelete, "Delete Action")
        tTip.SetToolTip(btnPlayPause, "Play Script")
        tTip.SetToolTip(btnStop, "Stop Script")

        AddHandler btnA.Click, AddressOf cb_Click
        AddHandler btnB.Click, AddressOf cb_Click
        AddHandler btnX.Click, AddressOf cb_Click
        AddHandler btnY.Click, AddressOf cb_Click
        AddHandler btnLS.Click, AddressOf cb_Click
        AddHandler btnLB.Click, AddressOf cb_Click
        AddHandler btnLT.Click, AddressOf cb_Click
        AddHandler btnRS.Click, AddressOf cb_Click
        AddHandler btnRB.Click, AddressOf cb_Click
        AddHandler btnRT.Click, AddressOf cb_Click
        AddHandler btnDD.Click, AddressOf cb_Click
        AddHandler btnDL.Click, AddressOf cb_Click
        AddHandler btnDR.Click, AddressOf cb_Click
        AddHandler btnDU.Click, AddressOf cb_Click
        AddHandler btnBack.Click, AddressOf cb_Click
        AddHandler btnStart.Click, AddressOf cb_Click
        AddHandler btnGuide.Click, AddressOf cb_Click
        AddHandler pbRS.MouseDown, AddressOf pb_MouseDown
        AddHandler pbLS.MouseDown, AddressOf pb_MouseDown
        AddHandler pbRS.DoubleClick, AddressOf pb_DoubleClick
        AddHandler pbLS.DoubleClick, AddressOf pb_DoubleClick
        AddHandler txtLSX.Validated, AddressOf txtTS_Validating
        AddHandler txtLSY.Validated, AddressOf txtTS_Validating
        AddHandler txtRSX.Validated, AddressOf txtTS_Validating
        AddHandler txtRSY.Validated, AddressOf txtTS_Validating
        AddHandler txtLT.Validated, AddressOf txtTrigger_Validating
        AddHandler txtRT.Validated, AddressOf txtTrigger_Validating

        AddHandler btnLT.Click, AddressOf cTrigger_Click
        AddHandler btnRT.Click, AddressOf cTrigger_Click

        clearScript()
        blankScript()

        txtController1.Text = GetSetting(Application.ProductName, "Settings", "Controller1", "10.100.8.51")
        txtController2.Text = GetSetting(Application.ProductName, "Settings", "Controller2", "10.100.8.52")
        txtController3.Text = GetSetting(Application.ProductName, "Settings", "Controller3", "10.100.8.53")
        txtController4.Text = GetSetting(Application.ProductName, "Settings", "Controller4", "CM")
        cbPrecompile.Checked = (GetSetting(Application.ProductName, "Settings", "Precompile", "True") = "True")
        cbTrace.Checked = (GetSetting(Application.ProductName, "Settings", "Trace", "True") = "True")
        cbSync.Checked = (GetSetting(Application.ProductName, "Settings", "Sync", "False") = "True")
        cbSyncWait.Checked = (GetSetting(Application.ProductName, "Settings", "SyncWait", "False") = "True")
        txtSync.Text = GetSetting(Application.ProductName, "Settings", "SyncIP", "97.82.214.2")
    End Sub

    Private Sub clearScript()
        rbPress.Checked = True
        valLS = New Point(-128, -128)
        valRS = New Point(-128, -128)
        valLT = -1
        valRT = -1
        drawPB(pbLS)
        drawPB(pbRS)
        refreshLT()
        refreshRT()
        cmbAdd.SelectedIndex = 0
        loopTarget = Nothing
        txtFlowTarget.Text = vbNullString
        filename = vbNullString
        txtGame.Text = vbNullString
        txtTitle.Text = vbNullString
        txtDesc.Text = vbNullString
        cbControllerIP.SelectedIndex = 0

        groups.Clear()
        lbGroups.Items.Clear()
        lbActions.Items.Clear()
    End Sub

    Private Sub blankScript()
        Dim g As New clsActionGroup(mainGroup)
        groups.Add(g.name, g)
        lbGroups.Items.Add(g)
        activeGroup = g
        lbGroups.SelectedItem = g
        refreshGroup()
    End Sub

    Private Sub cb_Click(sender As System.Object, e As System.EventArgs)
        Dim btn As Button = CType(sender, Button)
        btn.FlatAppearance.BorderSize = 1 - btn.FlatAppearance.BorderSize
    End Sub

    Private Sub cTrigger_Click(sender As System.Object, e As System.EventArgs)
        Dim btn As Button = CType(sender, Button)
        If btn Is btnLT Then
            valLT = IIf(btn.FlatAppearance.BorderSize, 255, -1)
            refreshLT()
        End If
        If btn Is btnRT Then
            valRT = IIf(btn.FlatAppearance.BorderSize, 255, -1)
            refreshRT()
        End If
    End Sub

    Private Sub refreshLT()
        txtLT.Text = IIf(valLT >= 0, valLT, vbNullString)
        btnLT.FlatAppearance.BorderSize = IIf(valLT >= 0, 1, 0)
    End Sub

    Private Sub refreshRT()
        txtRT.Text = IIf(valRT >= 0, valRT, vbNullString)
        btnRT.FlatAppearance.BorderSize = IIf(valRT >= 0, 1, 0)
    End Sub

    Private Sub drawPB(pb As PictureBox)
        Dim x As Integer = 0
        Dim y As Integer = 0
        If pb Is pbLS Then
            x = valLS.X
            y = valLS.Y
            txtLSX.Text = IIf(x > -128, x, vbNullString)
            txtLSY.Text = IIf(y > -128, y, vbNullString)
        End If
        If pb Is pbRS Then
            x = valRS.X
            y = valRS.Y
            txtRSX.Text = IIf(x > -128, x, vbNullString)
            txtRSY.Text = IIf(y > -128, y, vbNullString)
        End If
        Dim px As Integer = (CDec(x) + 147) * 52 / 254
        Dim py As Integer = (CDec(y) + 147) * 52 / 254
        With pb
            .Image = New Bitmap(.Width, .Height)
            Dim g As Graphics = Graphics.FromImage(.Image)
            g.Clear(.BackColor)
            g.DrawImage(.BackgroundImage, New Point((.Width - .BackgroundImage.Width) / 2 - 1, (.Height - .BackgroundImage.Height) / 2 - 1))
            g.DrawRectangle(Pens.White, New Rectangle(4, 4, .Width - 11, .Height - 11))
            If x > -128 And y > -128 Then g.FillRectangle(Brushes.Red, New Rectangle(px - 1, py - 1, 3, 3))
            g.Dispose()
            .Invalidate()
        End With
    End Sub

    Private Sub pb_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        Dim px As Integer
        Dim py As Integer
        If e.Button = Windows.Forms.MouseButtons.Right Then
            px = 30
            py = 30
        Else
            px = e.X
            py = e.Y
        End If
        If px > 56 Then px = 56
        If px < 4 Then px = 4
        If py > 56 Then py = 56
        If py < 4 Then py = 4

        Dim pb As PictureBox = CType(sender, PictureBox)
        If pb Is pbLS Then
            valLS.X = CDec(px) * 254 / 52 - 147
            valLS.Y = CDec(py) * 254 / 52 - 147
        End If
        If pb Is pbRS Then
            valRS.X = CDec(px) * 254 / 52 - 147
            valRS.Y = CDec(py) * 254 / 52 - 147
        End If
        drawPB(pb)
    End Sub

    Private Sub pb_DoubleClick(sender As Object, e As System.EventArgs)
        If sender Is pbLS Then
            valLS.X = -128
            valLS.Y = -128
        End If
        If sender Is pbRS Then
            valRS.X = -128
            valRS.Y = -128
        End If
        drawPB(sender)
    End Sub

    Private Sub txtTS_Validating(sender As Object, e As System.EventArgs)
        Dim txt As TextBox = CType(sender, TextBox)
        If Trim(txt.Text) = vbNullString Then
            txt.Text = vbNullString
            If txt Is txtLSX Or txt Is txtLSY Then
                valLS.X = -128
                valLS.Y = -128
                drawPB(pbLS)
            End If
            If txt Is txtRSX Or txt Is txtRSY Then
                valRS.X = -128
                valRS.Y = -128
                drawPB(pbRS)
            End If
            Exit Sub
        End If
        If Not IsNumeric(txt.Text) Then
            If txt Is txtLSX Then txt.Text = valLS.X
            If txt Is txtLSY Then txt.Text = valLS.Y
            If txt Is txtRSX Then txt.Text = valRS.X
            If txt Is txtRSY Then txt.Text = valRS.Y
        End If
        Dim val As Integer = Math.Round(CDec(txt.Text))
        If val < -127 Then val = -127
        If val > 127 Then val = 127
        If txt Is txtLSX Then
            valLS.X = val
            drawPB(pbLS)
        End If
        If txt Is txtLSY Then
            valLS.Y = val
            drawPB(pbLS)
        End If
        If txt Is txtRSX Then
            valRS.X = val
            drawPB(pbRS)
        End If
        If txt Is txtRSY Then
            valRS.Y = val
            drawPB(pbRS)
        End If
    End Sub

    Private Sub txtTrigger_Validating(sender As Object, e As System.EventArgs)
        Dim txt As TextBox = CType(sender, TextBox)
        If Trim(txt.Text) = vbNullString Then
            txt.Text = vbNullString
            If txt Is txtLT Then
                valLT = -1
                refreshLT()
            End If
            If txt Is txtRT Then
                valRT = -1
                refreshRT()
            End If
            Exit Sub
        End If
        If Not IsNumeric(txt.Text) Then
            If txt Is txtLT Then txt.Text = IIf(valLT > 0, valLT, vbNullString)
            If txt Is txtRT Then txt.Text = IIf(valRT > 0, valRT, vbNullString)
            Exit Sub
        End If
        Dim val As Integer = Math.Round(CDec(txt.Text))
        If val < 0 Then val = 0
        If val > 255 Then val = 255
        If txt Is txtLT Then
            valLT = val
            refreshLT()
        End If
        If txt Is txtRT Then
            valRT = val
            refreshRT()
        End If
    End Sub

    Private Sub rbPress_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbPress.CheckedChanged
        With rbPress
            If .Checked Then
                lblControllerHold.Visible = .Checked
                txtControllerHold.Visible = .Checked
                lblControllerWait.Visible = .Checked
                txtControllerWait.Visible = .Checked
                lblControllerRepeat.Visible = .Checked
                txtControllerRepeat.Visible = .Checked
            End If
        End With
    End Sub

    Private Sub rbRelease_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbRelease.CheckedChanged
        With rbRelease
            If .Checked Then
                lblControllerHold.Visible = Not .Checked
                txtControllerHold.Visible = Not .Checked
                lblControllerWait.Visible = Not .Checked
                txtControllerWait.Visible = Not .Checked
                lblControllerRepeat.Visible = Not .Checked
                txtControllerRepeat.Visible = Not .Checked
            End If
        End With
    End Sub

    Private Sub rbHold_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbHold.CheckedChanged
        With rbHold
            If .Checked Then
                lblControllerHold.Visible = Not .Checked
                txtControllerHold.Visible = Not .Checked
                lblControllerWait.Visible = Not .Checked
                txtControllerWait.Visible = Not .Checked
                lblControllerRepeat.Visible = Not .Checked
                txtControllerRepeat.Visible = Not .Checked
            End If
        End With
    End Sub

    Private Sub rbWait_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbWait.CheckedChanged
        With rbWait
            If .Checked Then
                lblFlowWait.Visible = .Checked
                txtFlowWait.Visible = .Checked
                lblFlowRepeat.Visible = Not .Checked
                txtFlowRepeat.Visible = Not .Checked
                lblFlowTarget.Visible = Not .Checked
                txtFlowTarget.Visible = Not .Checked
                btnFlowTarget.Visible = Not .Checked
            End If
        End With
    End Sub

    Private Sub rbLoop_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbLoop.CheckedChanged
        With rbLoop
            If .Checked Then
                lblFlowWait.Visible = Not .Checked
                txtFlowWait.Visible = Not .Checked
                lblFlowRepeat.Visible = .Checked
                txtFlowRepeat.Visible = .Checked
                lblFlowTarget.Visible = .Checked
                txtFlowTarget.Visible = .Checked
                btnFlowTarget.Visible = .Checked
            End If
        End With
    End Sub

    Private Sub rbGroup_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbGroup.CheckedChanged
        With rbGroup
            If .Checked Then
                lblFlowWait.Visible = Not .Checked
                txtFlowWait.Visible = Not .Checked
                lblFlowRepeat.Visible = .Checked
                txtFlowRepeat.Visible = .Checked
                lblFlowTarget.Visible = .Checked
                txtFlowTarget.Visible = .Checked
                btnFlowTarget.Visible = .Checked
            End If
        End With
    End Sub

    Private Sub rbInputVideo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbInputVideo.CheckedChanged
        With rbInputVideo
            If .Checked Then
                gbVideo.Visible = .Checked
            End If
        End With
    End Sub

    Private Sub rbInputAudio_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbInputAudio.CheckedChanged
        With rbInputAudio
            If .Checked Then
                gbVideo.Visible = Not .Checked
                MsgBox("Not yet supported.")
            End If
        End With
    End Sub

    Private Sub rbInputRumble_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbInputRumble.CheckedChanged
        With rbInputRumble
            If .Checked Then
                gbVideo.Visible = Not .Checked
                MsgBox("Not yet supported.")
            End If
        End With
    End Sub


    'code add
    'code update
    'code delete
    'code move up/down
    'load
    'save
    'play
    'stop
    'pause
    'continue
    'select controller

    'Edit ui:
    'Store path when loading, default to save as that
    'Entries for the 4 main instruction types
    'Add to (before/after/1st/last)
    'Delete selected
    'Internally goto should probably link to another instruction, and just display the line number on tostring
    'Would have tostring for display (with line #) and serialize to save to file
    'Script name
    'Save script
    'Load script
    'Test script (pick controller)
    'Stop script
    'Pause/continue script

    'Playback ui
    'Can open multiple windows
    'Sections have compact/expanded view
    'Controller section, pick method and port
    'Manual section, controller view.
    'Script section, pick script, view current position
    'Run section, stop/pause/continue/start/start all

    '"Compile" to list of just press and release events with links back to parent instructions. But how do we show when wait is active or goto or interleaved press & hold.
    'Waits could compile as null presses
    'Maybe just leave out loop updating?

    '    btnUp = &H100
    'btnDown = &H200
    'btnLeft = &H400
    'btnRight = &H800
    'btnStart = &H1000
    'btnBack = &H2000
    'btnL3 = &H4000
    'btnR3 = &H8000
    'btnLB = &H1
    'btnRB = &H2
    'btnGuide = &H4
    'btnA = &H10
    'btnB = &H20
    'btnX = &H40
    'btnY = &H80

    Public Function btnToMap() As Integer
        Return (btnDU.FlatAppearance.BorderSize * &H100) _
            Or (btnDD.FlatAppearance.BorderSize * &H200) _
            Or (btnDL.FlatAppearance.BorderSize * &H400) _
            Or (btnDR.FlatAppearance.BorderSize * &H800) _
            Or (btnStart.FlatAppearance.BorderSize * &H1000) _
            Or (btnBack.FlatAppearance.BorderSize * &H2000) _
            Or (btnLS.FlatAppearance.BorderSize * &H4000) _
            Or (btnRS.FlatAppearance.BorderSize * &H8000) _
            Or (btnLB.FlatAppearance.BorderSize * &H1) _
            Or (btnRB.FlatAppearance.BorderSize * &H2) _
            Or (btnGuide.FlatAppearance.BorderSize * &H4) _
            Or (btnA.FlatAppearance.BorderSize * &H10) _
            Or (btnB.FlatAppearance.BorderSize * &H20) _
            Or (btnX.FlatAppearance.BorderSize * &H40) _
            Or (btnY.FlatAppearance.BorderSize * &H80)
    End Function

    Public Sub mapToBtn(map As Integer)
        btnDU.FlatAppearance.BorderSize = IIf(map And &H100, 1, 0)
        btnDD.FlatAppearance.BorderSize = IIf(map And &H200, 1, 0)
        btnDL.FlatAppearance.BorderSize = IIf(map And &H400, 1, 0)
        btnDR.FlatAppearance.BorderSize = IIf(map And &H800, 1, 0)
        btnStart.FlatAppearance.BorderSize = IIf(map And &H1000, 1, 0)
        btnBack.FlatAppearance.BorderSize = IIf(map And &H2000, 1, 0)
        btnLS.FlatAppearance.BorderSize = IIf(map And &H4000, 1, 0)
        btnRS.FlatAppearance.BorderSize = IIf(map And &H8000, 1, 0)
        btnLB.FlatAppearance.BorderSize = IIf(map And &H1, 1, 0)
        btnRB.FlatAppearance.BorderSize = IIf(map And &H2, 1, 0)
        btnGuide.FlatAppearance.BorderSize = IIf(map And &H4, 1, 0)
        btnA.FlatAppearance.BorderSize = IIf(map And &H10, 1, 0)
        btnB.FlatAppearance.BorderSize = IIf(map And &H20, 1, 0)
        btnX.FlatAppearance.BorderSize = IIf(map And &H40, 1, 0)
        btnY.FlatAppearance.BorderSize = IIf(map And &H80, 1, 0)
    End Sub

    Private Function createAction() As clsAction
        Dim action As clsAction = Nothing
        If tcActions.SelectedTab Is tpController Then
            If rbRelease.Checked Then action = New clsActionRelease(cbControllerIP.SelectedItem, btnToMap, valLT, valRT, valLS, valRS, activeGroup)
            If rbHold.Checked Then action = New clsActionHold(cbControllerIP.SelectedItem, btnToMap, valLT, valRT, valLS, valRS, activeGroup)
            If rbPress.Checked Then action = New clsActionPress(cbControllerIP.SelectedItem, btnToMap, valLT, valRT, valLS, valRS, unformatMS(txtControllerHold.Text), unformatMS(txtControllerWait.Text), CInt(txtControllerRepeat.Text), activeGroup)
        ElseIf tcActions.SelectedTab Is tpFlow Then
            If rbWait.Checked Then action = New clsActionWait(unformatMS(txtFlowWait.Text), activeGroup)
            If rbLoop.Checked Then action = New clsActionLoop(loopTarget, CInt(txtFlowRepeat.Text), activeGroup)
            If rbGroup.Checked Then action = New clsActionAGroup(groupTarget, CInt(txtFlowRepeat.Text), activeGroup)
        ElseIf tcActions.SelectedTab Is tpInput Then
            If rbInputVideo.Checked Then action = New clsActionInputVideo(unformatMS(txtInputInterval.Text), unformatMS(txtInputDuration.Text), New Point(txtVideoPixelX.Text, txtVideoPixelY.Text), btnVideoColorMin.BackColor, btnVideoColorMax.BackColor, activeGroup)
            'If rbInputAudio.Checked Then action = New clsActionInputAudio(txtInputInterval.Text, txtInputDuration.Text, <source>,<minvol>,<maxvol>)
            'If rbInputRumble.Checked Then action = New clsActionInputRumble(txtInputInterval.Text, txtInputDuration.Text, <controller>,<rumbletype>,<minval>,<maxval>)
            'todo: create input actions
        End If
        Return action
    End Function

    Private Sub btnAdd_Click(sender As System.Object, e As System.EventArgs) Handles btnAdd.Click
        Dim action As clsAction = createAction()

        Dim index As Integer = activeGroup.actions.Count
        Select Case Me.cmbAdd.Text
            Case "Last"
            Case "After"
                If lbActions.SelectedItems.Count > 0 Then
                    index = lbActions.SelectedItems(lbActions.SelectedItems.Count - 1).index + 1
                Else
                    index = lbActions.Items.Count
                End If
            Case "Before"
                If lbActions.SelectedItems.Count > 0 Then
                    index = lbActions.SelectedItems(0).index
                Else
                    index = 0
                End If
            Case "First"
                index = 0
        End Select

        action.index = index
        activeGroup.actions.Insert(index, action)
        lbActions.Items.Insert(index, action)
        For i As Integer = index + 1 To activeGroup.actions.Count - 1
            activeGroup.actions(i).index += 1
            lbActions.Items(i).Refresh(lbActions)
        Next
    End Sub

    Private Sub txtNumeric_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtFlowRepeat.Validating, txtControllerRepeat.Validating, txtVideoPixelX.Validating, txtVideoPixelY.Validating
        Dim re As New Regex("^\d+$")
        If Not re.IsMatch(sender.text) Then
            e.Cancel = True
            MsgBox("You may only enter numbers in this field.")
        End If
    End Sub

    Public Function unformatMS(src As String) As Integer
        Dim re As New Regex("^((((?<d>\d+)d)?((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?((?<ms>\d+)ms)?)|((?<h>\d+):(?<m>\d\d):(?<s>\d\d)(\.(?<msd>\d{1,3}))?)|((?<m>\d+):(?<s>\d\d)(\.(?<msd>\d{1,3}))?)|((?<s>\d+)?(\.(?<msd>\d{1,3}))?))$")
        Dim m As System.Text.RegularExpressions.Match = re.Match(src)
        If Not m.Success Then Return -1
        Dim strMs As String = m.Groups("msd").Value
        If strMs = vbNullString Then strMs = m.Groups("ms").Value Else strMs = strMs.PadRight(3, "0")
        If strMs = vbNullString Then strMs = 0
        Dim strS As String = m.Groups("s").Value
        If strS = vbNullString Then strS = 0
        Dim strM As String = m.Groups("m").Value
        If strM = vbNullString Then strM = 0
        Dim strH As String = m.Groups("h").Value
        If strH = vbNullString Then strH = 0
        Dim strD As String = m.Groups("d").Value
        If strD = vbNullString Then strD = 0

        Dim ts As New TimeSpan(strD, strH, strM, strS, strMs)
        Return Math.Floor(ts.TotalMilliseconds)
    End Function

    Private Sub txtTime_Validation(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtControllerWait.Validating, txtControllerHold.Validating, txtFlowWait.Validating, txtInputInterval.Validating, txtInputDuration.Validating
        Dim ms As Long = unformatMS(sender.text)
        If ms = -1 Then
            e.Cancel = True
            MsgBox("You may only enter times in this field." & vbCrLf & "Valid formats are:" & vbCrLf & _
                    "1:02:03.004" & vbCrLf & _
                    "62:03.004" & vbCrLf & _
                    "3723.004" & vbCrLf & _
                    "1h2m3s4ms" & vbCrLf & _
                    "62m3s4ms" & vbCrLf & _
                    "3723s4ms" & vbCrLf & _
                    "3723004ms" & vbCrLf)
        End If
        sender.text = formatMS(ms)
    End Sub

    Private Sub btnTgt_Click(sender As System.Object, e As System.EventArgs) Handles btnFlowTarget.Click
        If rbLoop.Checked Then
            loopTarget = lbActions.SelectedItem
            txtFlowTarget.Text = loopTarget.index + 1
        End If
        If rbGroup.Checked Then
            groupTarget = lbGroups.SelectedItem
            If groupTarget Is activeGroup OrElse groupTarget.containsGroup(activeGroup) Then
                MsgBox("A group can't link to itself.")
                groupTarget = Nothing
                txtFlowTarget.Text = vbNullString
            Else
                txtFlowTarget.Text = groupTarget.name
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click
        If lbActions.SelectedItems.Count = 0 Then Exit Sub
        Dim lstRemove As New List(Of clsAction)
        For Each action As clsAction In lbActions.SelectedItems
            lstRemove.Add(action)
        Next
        For Each action In lstRemove
            activeGroup.actions.Remove(action)
            lbActions.Items.Remove(action)
            action.delete()
        Next
        For i = lstRemove(0).index To activeGroup.actions.Count - 1
            activeGroup.actions(i).index = i
        Next
        For i = lstRemove(0).index To activeGroup.actions.Count - 1
            lbActions.Items(i).refresh(lbActions)
        Next

    End Sub

    Private Sub btnMoveUp_Click(sender As System.Object, e As System.EventArgs) Handles btnMoveUp.Click
        'any internals are moved after, then move it before the one previous (if not at beginning)
        If lbActions.SelectedItems.Count = 0 Then Exit Sub
        Dim startindex As Integer = lbActions.SelectedItems(0).index
        If startindex < 1 Then Exit Sub
        Dim endIndex As Integer = lbActions.SelectedItems(lbActions.SelectedItems.Count - 1).index
        Dim lstMove As New List(Of clsAction)
        For Each action As clsAction In lbActions.SelectedItems
            lstMove.Add(action)
        Next
        For Each action In lstMove
            activeGroup.actions.Remove(action)
            lbActions.Items.Remove(action)
        Next
        For i As Integer = lstMove.Count - 1 To 0 Step -1
            activeGroup.actions.Insert(startindex - 1, lstMove(i))
            lbActions.Items.Insert(startindex - 1, lstMove(i))
        Next
        For i As Integer = startindex - 1 To endIndex
            activeGroup.actions(i).index = i
            lbActions.Items(i).refresh(lbActions)
        Next
        For Each action In lstMove
            lbActions.SelectedItems.Add(action)
        Next
    End Sub

    Private Sub btnMoveDown_Click(sender As System.Object, e As System.EventArgs) Handles btnMoveDown.Click
        If lbActions.SelectedItems.Count = 0 Then Exit Sub
        Dim endIndex As Integer = lbActions.SelectedItems(lbActions.SelectedItems.Count - 1).index
        If endIndex > lbActions.Items.Count - 2 Then Exit Sub
        Dim startindex As Integer = lbActions.SelectedItems(0).index
        Dim lstMove As New List(Of clsAction)
        For Each action As clsAction In lbActions.SelectedItems
            lstMove.Add(action)
        Next
        For Each action In lstMove
            activeGroup.actions.Remove(action)
            lbActions.Items.Remove(action)
        Next
        For i As Integer = 0 To lstMove.Count - 1
            activeGroup.actions.Insert(startindex + i + 1, lstMove(i))
            lbActions.Items.Insert(startindex + i + 1, lstMove(i))
        Next
        For i As Integer = startindex To endIndex + 1
            activeGroup.actions(i).index = i
            lbActions.Items(i).refresh(lbActions)
        Next
        For Each action In lstMove
            lbActions.SelectedItems.Add(action)
        Next
    End Sub

    Private Sub lbActions_DoubleClick(sender As Object, e As System.EventArgs) Handles lbActions.DoubleClick
        If lbActions.SelectedItems.Count <> 1 Then Exit Sub
        Dim action As clsAction = lbActions.SelectedItem
        Select Case action.getActType
            Case ActionType.actWait
                Dim aWait As clsActionWait = action
                tcActions.SelectedTab = tpFlow
                rbWait.Checked = True
                With aWait
                    txtFlowWait.Text = formatMS(.delay)
                End With
            Case ActionType.actLoop
                Dim aLoop As clsActionLoop = action
                tcActions.SelectedTab = tpFlow
                rbLoop.Checked = True
                With aLoop
                    txtFlowRepeat.Text = .repeat
                    loopTarget = .target
                    txtFlowTarget.Text = .target.index + 1
                End With
            Case ActionType.actGroup
                Dim aGroup As clsActionAGroup = action
                tcActions.SelectedTab = tpFlow
                rbGroup.Checked = True
                With aGroup
                    txtFlowRepeat.Text = .repeat
                    groupTarget = .target
                    txtFlowTarget.Text = .target.name
                End With
            Case ActionType.actHold
                Dim aHold As clsActionHold = action
                tcActions.SelectedTab = tpController
                rbHold.Checked = True
                With aHold
                    mapToBtn(.buttonMask)
                    valLT = IIf(.LTDefined, .LT, -1)
                    refreshLT()
                    valRT = IIf(.RTDefined, .RT, -1)
                    refreshRT()
                    If .LS.X = -128 Or .LS.Y = -128 Then
                        valLS.X = -128
                        valLS.Y = -128
                    Else
                        valLS.X = .LS.X
                        valLS.Y = .LS.Y
                    End If
                    drawPB(pbLS)
                    If .RS.X = -128 Or .RS.Y = -128 Then
                        valRS.X = -128
                        valRS.Y = -128
                    Else
                        valRS.X = .RS.X
                        valRS.Y = .RS.Y
                    End If
                    drawPB(pbRS)
                    cbControllerIP.SelectedIndex = aHold.controllerNumber - 1
                End With
            Case ActionType.actPress
                Dim aPress As clsActionPress = action
                tcActions.SelectedTab = tpController
                rbPress.Checked = True
                With aPress
                    mapToBtn(.buttonMask)
                    valLT = IIf(.LTDefined, .LT, -1)
                    refreshLT()
                    valRT = IIf(.RTDefined, .RT, -1)
                    refreshRT()
                    If .LS.X = -128 Or .LS.Y = -128 Then
                        valLS.X = -128
                        valLS.Y = -128
                    Else
                        valLS.X = .LS.X
                        valLS.Y = .LS.Y
                    End If
                    drawPB(pbLS)
                    If .RS.X = -128 Or .RS.Y = -128 Then
                        valRS.X = -128
                        valRS.Y = -128
                    Else
                        valRS.X = .RS.X
                        valRS.Y = .RS.Y
                    End If
                    drawPB(pbRS)
                    txtControllerRepeat.Text = .repeat
                    txtControllerWait.Text = formatMS(.waitTime)
                    txtControllerHold.Text = formatMS(.holdTime)
                End With
                cbControllerIP.SelectedIndex = aPress.controllerNumber - 1
            Case ActionType.actRelease
                Dim aRelease As clsActionRelease = action
                tcActions.SelectedTab = tpController
                rbRelease.Checked = True
                With aRelease
                    mapToBtn(.buttonMask)
                    valLT = IIf(.LTDefined, 0, -1)
                    refreshLT()
                    valRT = IIf(.RTDefined, 0, -1)
                    refreshRT()
                End With
                cbControllerIP.SelectedIndex = aRelease.controllerNumber - 1
            Case ActionType.actInputVideo
                Dim aInputVideo As clsActionInputVideo = action
                tcActions.SelectedTab = tpInput
                With aInputVideo
                    txtInputInterval.Text = formatMS(.interval)
                    txtInputDuration.Text = formatMS(.duration)
                    txtVideoPixelX.Text = .pixel.X
                    txtVideoPixelY.Text = .pixel.Y
                    btnVideoColorMin.BackColor = .minColor
                    btnVideoColorMax.BackColor = .maxColor
                End With
            Case ActionType.actInputAudio
                tcActions.SelectedTab = tpInput
                'todo: populate control
            Case ActionType.actInputRumble
                tcActions.SelectedTab = tpInput
                'todo: populate control
        End Select
    End Sub

    Private Sub btnApply_Click(sender As System.Object, e As System.EventArgs) Handles btnApply.Click
        If lbActions.SelectedItems.Count <> 1 Then Exit Sub
        Dim action As clsAction = createAction()
        Dim oldAction As clsAction = lbActions.SelectedItem
        action.index = oldAction.index
        For Each referrer In oldAction.referrers
            referrer.target = action
            action.referrers.Add(referrer)
        Next
        activeGroup.actions(lbActions.SelectedIndex) = action
        lbActions.Items(lbActions.SelectedIndex) = action
        action.refresh(lbActions)
        oldAction.referrers.Clear()
        oldAction.delete()
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles NewToolStripMenuItem.Click
        clearScript()
        blankScript()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        fdOpen.FileName = vbNullString
        fdOpen.InitialDirectory = IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\scripts"
        fdOpen.ShowDialog()
        If fdOpen.FileName = vbNullString Then Exit Sub
        filename = fdOpen.FileName
        loadScript(filename)
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If filename = vbNullString Then
            SaveAsToolStripMenuItem_Click(sender, e)
            Exit Sub
        End If
        saveScriptXML(filename)
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        fdSave.InitialDirectory = IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\scripts"
        If filename <> vbNullString Then
            fdSave.FileName = filename
        Else
            If txtGame.Text = vbNullString Then
                If txtTitle.Text = vbNullString Then
                    fdSave.FileName = vbNullString
                Else
                    fdSave.FileName = fdSave.InitialDirectory & "\" & txtTitle.Text & ".axb"
                End If
            Else
                If txtTitle.Text = vbNullString Then
                    fdSave.FileName = fdSave.InitialDirectory & "\" & txtGame.Text & ".axb"
                Else
                    fdSave.FileName = fdSave.InitialDirectory & "\" & txtGame.Text & " - " & txtTitle.Text & ".axb"
                End If
            End If
        End If
        fdSave.ShowDialog()
        If fdSave.FileName = vbNullString Then Exit Sub
        filename = fdSave.FileName
        saveScriptXML(filename)
    End Sub

    'Private Sub saveScript(path As String)
    '    Dim sb As New System.Text.StringBuilder
    '    sb.AppendLine("#" & txtGame.Text)
    '    sb.AppendLine("#" & txtTitle.Text)
    '    For Each line As String In txtDesc.Lines
    '        sb.AppendLine("#" & line)
    '    Next
    '    For Each action As clsAction In lbActions.Items
    '        sb.AppendLine(action.serialize)
    '    Next
    '    IO.File.WriteAllText(path, sb.ToString)
    'End Sub

    Private Sub saveScriptXML(path As String)
        Dim doc As New XmlDocument
        Dim root As XmlElement = doc.CreateElement("XBScript")
        doc.AppendChild(root)
        Dim desc As XmlElement = doc.CreateElement("Information")
        root.AppendChild(desc)
        desc.AppendChild(doc.CreateElement("Game")).InnerText = txtGame.Text
        desc.AppendChild(doc.CreateElement("Title")).InnerText = txtTitle.Text
        desc.AppendChild(doc.CreateElement("Description")).InnerText = Join(txtDesc.Lines, vbCrLf)
        Dim agsNode As XmlElement = doc.CreateElement("ActionGroups")
        root.AppendChild(agsNode)
        For Each group As clsActionGroup In groups.Values
            Dim agNode As XmlElement = doc.CreateElement("ActionGroup")
            agsNode.AppendChild(agNode)
            agNode.AppendChild(doc.CreateElement("Name")).InnerText = group.name
            For Each action As clsAction In group.actions
                agNode.AppendChild(action.toXML(doc))
            Next
        Next
        IO.File.WriteAllText(path, doc.OuterXml)
    End Sub

    Private Sub loadScript(path As String)
        clearScript()
        loadScriptXML(path)
        For Each g As clsActionGroup In groups.Values
            lbGroups.Items.Add(g)
        Next
        If groups.ContainsKey(mainGroup) Then activeGroup = groups(mainGroup) Else activeGroup = lbGroups.Items(0)
        lbGroups.SelectedItem = activeGroup
        linkActions()
        refreshGroup()
    End Sub

    Private Sub linkActions()
        For Each group In groups.Values
            For Each action As clsAction In group.actions
                If action.getActType = ActionType.actLoop Then
                    Dim aLoop As clsActionLoop = action
                    aLoop.linkTarget(group.actions)
                End If
                If action.getActType = ActionType.actGroup Then
                    Dim aGroup As clsActionAGroup = action
                    aGroup.linkTarget(groups)
                End If
            Next
        Next
    End Sub

    Private Sub refreshGroup()
        lbGroups.SelectedItem = activeGroup
        lbActions.Items.Clear()
        txtFlowTarget.Text = vbNullString
        groupTarget = Nothing
        loopTarget = Nothing
        For Each a As clsAction In activeGroup.actions
            lbActions.Items.Add(a)
        Next
    End Sub

    Private Sub loadScriptXML(path As String)
        Dim doc As New XmlDocument
        Try
            doc.Load(path)
        Catch ex As XmlException
            loadScriptLegacy(path)
            Exit Sub
        End Try
        Dim node As Xml.XmlNode = doc.SelectSingleNode("/XBScript/Information/Game")
        If Not node Is Nothing Then txtGame.Text = node.InnerText
        node = doc.SelectSingleNode("/XBScript/Information/Title")
        If Not node Is Nothing Then txtTitle.Text = node.InnerText
        node = doc.SelectSingleNode("/XBScript/Information/Description")
        If Not node Is Nothing Then txtDesc.Text = node.InnerText
        Dim actionGroups As Xml.XmlNodeList = doc.SelectNodes("/XBScript/ActionGroups/ActionGroup")
        For Each agNode As Xml.XmlNode In actionGroups
            Dim ag As New clsActionGroup(agNode.SelectSingleNode("Name").InnerText)
            For Each actNode As Xml.XmlNode In agNode.ChildNodes
                If actNode.Name <> "Name" Then
                    Dim action As clsAction = clsAction.fromXML(actNode, ag)
                    action.index = ag.actions.Count
                    ag.actions.Add(action)
                End If

            Next
            groups.Add(ag.name, ag)
        Next
        filename = path
    End Sub

    Private Sub loadScriptLegacy(path As String)
        Dim lines() As String = IO.File.ReadAllLines(path)
        Dim header As New List(Of String)
        Dim start As Integer = 0
        While lines(start).StartsWith("#")
            header.Add(lines(start).Substring(1))
            start = start + 1
        End While

        If header.Count > 0 Then
            txtGame.Text = header(0)
            header.RemoveAt(0)
        End If

        If header.Count > 0 Then
            txtTitle.Text = header(0)
            header.RemoveAt(0)
        End If

        If header.Count > 0 Then txtDesc.Lines = header.ToArray

        Dim group As clsActionGroup = New clsActionGroup(mainGroup)
        groups.Add(group.name, group)

        For i = start To lines.Length - 1
            Dim action As clsAction = clsAction.deSerialize(lines(i), group)
            action.index = group.actions.Count
            group.actions.Add(action)
        Next
    End Sub

    Private Function formatMS(ms As Integer) As String
        Dim s As New TimeSpan(CLng(ms) * 10000)
        Dim sb As New System.Text.StringBuilder
        If s.Days > 0 Then sb.Append(Math.Floor(s.TotalDays) & "d")
        If s.Hours = 0 And s.Minutes = 0 And s.Seconds = 0 And s.Milliseconds = 0 Then Return sb.ToString
        If sb.Length > 0 OrElse s.Hours > 0 Then sb.Append(s.Hours.ToString(IIf(sb.Length = 0, "0", "00")) & "h")
        If s.Minutes = 0 And s.Seconds = 0 And s.Milliseconds = 0 Then Return sb.ToString
        If sb.Length > 0 OrElse s.Minutes > 0 Then sb.Append(s.Minutes.ToString(IIf(sb.Length = 0, "0", "00")) & "m")
        If s.Seconds = 0 And s.Milliseconds = 0 Then Return sb.ToString
        If sb.Length > 0 OrElse s.Seconds > 0 Then sb.Append(s.Seconds.ToString(IIf(sb.Length = 0, "0", "00")) & "s")
        If s.Milliseconds = 0 Then Return sb.ToString
        If sb.Length > 0 OrElse s.Milliseconds > 0 Then sb.Append(s.Milliseconds.ToString(IIf(sb.Length = 0, "0", "000")) & "ms")
        Return sb.ToString
    End Function

    Private Sub btnPlayPause_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayPause.Click
        If activeScript Is Nothing Then
            If Not activeGroup Is groups(mainGroup) Then
                activeGroup = groups(mainGroup)
                refreshGroup()
            End If
            Dim actions As New Generic.List(Of clsAction)
            For Each action As clsAction In activeGroup.actions
                actions.Add(action)
                If action.getActType = ActionType.actGroup Then
                    Dim agAction As clsActionAGroup = action
                    For i As Integer = 1 To agAction.repeat
                        actions.AddRange(agAction.target.getActions())
                    Next
                End If
            Next
            tmrScriptStatus.Enabled = True
            btnStop.Enabled = True
            Dim controllerIPS As New Dictionary(Of Byte, String)
            If txtController1.Text <> vbNullString Then controllerIPS.Add(1, txtController1.Text)
            If txtController2.Text <> vbNullString Then controllerIPS.Add(2, txtController2.Text)
            If txtController3.Text <> vbNullString Then controllerIPS.Add(3, txtController3.Text)
            If txtController4.Text <> vbNullString Then controllerIPS.Add(4, txtController4.Text)
            activeScript = New clsScript(actions, controllerIPS)
            If activeScript.state = clsScript.scriptState.scriptError Then
                activeScript = Nothing
                tmrScriptStatus.Enabled = False
                btnStop.Enabled = False
                Exit Sub
            End If
            Dim totalms As Integer = activeScript.stateActions(activeScript.stateActions.Count - 1).timeoffset
            Dim timeString As String = formatMS(totalms)
            If cbPrecompile.Checked Then
                If MsgBox("Ready to Start." & vbCrLf & "Runtime: " & timeString, vbOKCancel) = MsgBoxResult.Cancel Then
                    activeScript.dispose()
                    activeScript = Nothing
                    tmrScriptStatus.Enabled = False
                    btnStop.Enabled = False
                    Exit Sub
                End If
            End If
            If cbSyncWait.Checked Then
                Dim receivingUdpClient As New System.Net.Sockets.UdpClient(12345)
                Dim RemoteIpEndPoint As New System.Net.IPEndPoint(System.Net.IPAddress.Any, 0)
                Dim SyncStart As Boolean = False
                Do Until SyncStart
                    Dim receiveBytes As [Byte]() = receivingUdpClient.Receive(RemoteIpEndPoint)
                    If System.Text.Encoding.ASCII.GetString(receiveBytes) = "AutoGH" Then SyncStart = True
                Loop
                RemoteIpEndPoint = Nothing
                receivingUdpClient.Close()
                receivingUdpClient = Nothing
            End If
            If cbSync.Checked Then
                Dim sck As Net.Sockets.Socket
                sck = New Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Dgram, Net.Sockets.ProtocolType.Udp)
                sck.Connect(txtSync.Text, 12345)
                sck.Send(System.Text.Encoding.ASCII.GetBytes("AutoGH"))
                sck.Close()
            End If
            activeScript.startScript()
            setBtnPP(False)
        Else
            Select Case activeScript.state
                Case clsScript.scriptState.finished
                    activeScript.startScript()
                    setBtnPP(False)
                Case clsScript.scriptState.paused
                    activeScript.continueScript()
                    setBtnPP(False)
                Case clsScript.scriptState.ready
                    activeScript.startScript()
                    setBtnPP(False)
                Case clsScript.scriptState.running
                    activeScript.pauseScript()
                    setBtnPP(True)
            End Select
        End If
    End Sub

    Private Sub setBtnPP(play As Boolean)
        btnPlayPause.Text = IIf(play, "4", ";")
        btnPlayPause.ForeColor = IIf(play, Color.Green, Color.Goldenrod)
    End Sub

    Private Sub btnStop_Click(sender As System.Object, e As System.EventArgs) Handles btnStop.Click
        btnStop.Enabled = False
        setBtnPP(True)
        lblWaitTime.Text = vbNullString
        tmrScriptStatus.Enabled = False
        If Not activeScript Is Nothing Then activeScript.stopScript()
        activeScript = Nothing
    End Sub

    Private Sub tmrScriptStatus_Tick(sender As System.Object, e As System.EventArgs) Handles tmrScriptStatus.Tick
        Static lastAction As clsAction = Nothing
        If activeScript Is Nothing Then
            tmrScriptStatus.Enabled = False
            Exit Sub
        End If
        Dim asState As clsScript.scriptState = System.Threading.Thread.VolatileRead(activeScript.state)

        Select Case asState
            Case clsScript.scriptState.ready, clsScript.scriptState.paused
            Case clsScript.scriptState.running
                If cbTrace.Checked Then
                    Dim asAction As clsAction = System.Threading.Thread.VolatileRead(activeScript.lastAction)
                    Dim asWait As Integer = System.Threading.Thread.VolatileRead(activeScript.totalWait)
                    If Not asAction Is lastAction Then
                        lastAction = asAction
                        If Not lastAction Is Nothing Then
                            If Not lastAction.group Is activeGroup Then
                                activeGroup = lastAction.group
                                If activeGroup Is Nothing Then Stop
                                lbGroups.SelectedItem = activeGroup
                                activeGroup = lbGroups.SelectedItem
                                refreshGroup()
                            End If
                            lbActions.SelectedItems.Clear()
                            lbActions.SelectedItem = lastAction.original
                        End If
                    End If
                    lblWaitTime.Text = formatMS(asWait)
                End If
            Case clsScript.scriptState.finished
                btnStop.Enabled = False
                setBtnPP(True)
                activeScript = Nothing
                lblWaitTime.Text = vbNullString
                tmrScriptStatus.Enabled = False
        End Select
    End Sub

    Private Sub SongToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SongToolStripMenuItem.Click
        Dim ss As New frmMusic
        ss.ShowDialog()
        If Not ss.actions Is Nothing Then
            clearScript()
            blankScript()
            For Each act As clsAction In ss.actions
                act.group = activeGroup
                activeGroup.actions.Add(act)
            Next
            refreshGroup()
            Dim sb As New System.Text.StringBuilder
            sb.AppendLine("Song: " & ss.cbSong.SelectedItem.ToString() & " (" & ss.cbGame.SelectedItem.ToString() & ")" & IIf(ss.info <> vbNullString, " - #" & ss.info, ""))
            Dim players As New List(Of String)
            If Not ss.cbTrack0.SelectedItem Is Nothing Then players.Add("#1 " & ss.cbTrack0.SelectedItem.ToString() & " [" & ss.cbLevel0.SelectedItem.ToString() & "]")
            If Not ss.cbTrack1.SelectedItem Is Nothing Then players.Add("#2 " & ss.cbTrack1.SelectedItem.ToString() & " [" & ss.cbLevel1.SelectedItem.ToString() & "]")
            If Not ss.cbTrack2.SelectedItem Is Nothing Then players.Add("#3 " & ss.cbTrack2.SelectedItem.ToString() & " [" & ss.cbLevel2.SelectedItem.ToString() & "]")
            If Not ss.cbTrack3.SelectedItem Is Nothing Then players.Add("#4 " & ss.cbTrack3.SelectedItem.ToString() & " [" & ss.cbLevel3.SelectedItem.ToString() & "]")
            sb.AppendLine("Players: " & Join(players.ToArray, ", "))
            sb.AppendLine()
            sb.AppendLine("To use, install the game to a hard disk/USB stick to make load times consistent, start song manually, then choose to restart, move cursor to accept restart and then begin script, which will press the button to restart and then begin playing.")
            txtDesc.Text = sb.ToString
        End If
        ss.Dispose()
        ss = Nothing
    End Sub

    Private Sub btnRenameGroup_Click(sender As System.Object, e As System.EventArgs) Handles btnRenameGroup.Click
        If CType(lbGroups.SelectedItem, clsActionGroup).name = mainGroup Then
            MsgBox("Can't rename the main action group.")
            Exit Sub
        End If
        Dim ag As clsActionGroup = lbGroups.SelectedItem
        Dim name As String = InputBox("Enter new name:", , ag.name)
        If name = vbNullString Then Exit Sub
        For Each group As clsActionGroup In groups.Values
            If group.name.ToUpper() = name.ToUpper() Then
                If group Is ag Then Exit Sub
                MsgBox("That name is already in use.")
                Exit Sub
            End If
        Next

        groups.Remove(ag.name)
        ag.name = name
        groups.Add(ag.name, ag)
        lbGroups.RefreshItem(lbGroups.SelectedIndex)
    End Sub

    Private Sub btnAddGroup_Click(sender As System.Object, e As System.EventArgs) Handles btnAddGroup.Click
        Dim name As String = InputBox("Enter name of new group:")
        If name = vbNullString Then Exit Sub
        For Each group As clsActionGroup In groups.Values
            If group.name.ToUpper() = name.ToUpper() Then
                MsgBox("That name is already in use.")
                Exit Sub
            End If
        Next
        Dim ag As New clsActionGroup(name)
        lbGroups.Items.Add(ag)
        groups.Add(ag.name, ag)
    End Sub

    Private Sub btnDeleteGroup_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteGroup.Click
        If CType(lbGroups.SelectedItem, clsActionGroup).name = mainGroup Then
            MsgBox("Can't delete the main action group.")
            Exit Sub
        End If
        Dim ag As clsActionGroup = lbGroups.SelectedItem
        For Each group As clsActionGroup In groups.Values
            If (Not group Is ag) AndAlso group.containsGroup(ag) Then
                MsgBox("This group is used by the group """ & group.name & """ and cannot be deleted.")
                Exit Sub
            End If
        Next
        groups.Remove(ag.name)
        lbGroups.Items.Remove(ag)
    End Sub

    Private Sub lbGroups_DoubleClick(sender As Object, e As System.EventArgs) Handles lbGroups.DoubleClick
        If lbGroups.SelectedItem Is Nothing Then Exit Sub
        activeGroup = lbGroups.SelectedItem
        refreshGroup()
    End Sub

    Private Sub PatternToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PatternToolStripMenuItem.Click
        Dim ss As New frmPattern
        ss.ShowDialog()
        If Not ss.actions Is Nothing Then
            For Each act As clsAction In ss.actions
                act.group = activeGroup
                activeGroup.actions.Add(act)
            Next
            refreshGroup()
        End If
        ss.Dispose()
        ss = Nothing
    End Sub

    Private Sub BridgeModeToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles BridgeModeToolStripMenuItem.Click
        saveSettings()
        Dim bf As New frmBridge
        bf.ShowDialog(Me)
        bf.Dispose()
        bf = Nothing
    End Sub

    Private Sub CronusIdentifyToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CronusIdentifyToolStripMenuItem.Click
        Dim cmID As New frmCMIdentify
        cmID.ShowDialog(Me)
        cmID.Dispose()
        cmID = Nothing
    End Sub

    Private Sub validateController(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtController1.Validating, txtController2.Validating, txtController3.Validating, txtController4.Validating
        Dim txt As TextBox = CType(sender, TextBox)
        Dim re As New Regex("^((COM\d+)|(CM\d+)|(CM)|(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})|)$")
        If Not re.IsMatch(txt.Text) Then
            e.Cancel = True
            MsgBox("Invalid controller target")
        End If
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExportToolStripMenuItem.Click
        fdSave.Filter = "AutoXB Net Scripts|*.axn"
        fdSave.ShowDialog()
        If fdSave.FileName <> vbNullString Then
            If Not activeGroup Is groups(mainGroup) Then
                activeGroup = groups(mainGroup)
                refreshGroup()
            End If
            Dim actions As New Generic.List(Of clsAction)
            For Each action As clsAction In activeGroup.actions
                actions.Add(action)
                If action.getActType = ActionType.actGroup Then
                    Dim agAction As clsActionAGroup = action
                    For i As Integer = 1 To agAction.repeat
                        actions.AddRange(agAction.target.getActions())
                    Next
                End If
            Next
            tmrScriptStatus.Enabled = True
            btnStop.Enabled = True
            Dim controllerIPS As New Dictionary(Of Byte, String)
            If txtController1.Text <> vbNullString Then controllerIPS.Add(1, txtController1.Text)
            If txtController2.Text <> vbNullString Then controllerIPS.Add(2, txtController2.Text)
            If txtController3.Text <> vbNullString Then controllerIPS.Add(3, txtController3.Text)
            If txtController4.Text <> vbNullString Then controllerIPS.Add(4, txtController4.Text)
            Dim expScript As New clsScript(actions, controllerIPS)
            Dim sb As New System.Text.StringBuilder
            For Each sa As clsStatelessAction In expScript.stateActions
                sb.AppendLine(sa.ToString)
            Next
            Stop
        End If
        fdSave.Filter = "AutoXB Scripts|*.axb"
    End Sub

    Private Sub CaptureCardToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CaptureCardToolStripMenuItem.Click
        Dim frmCC As New frmCaptureCard
        frmCC.ShowDialog()
        frmCC.Dispose()
        frmCC = Nothing
    End Sub

    Private Sub CapwizToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs)
        Dim frmCW As New frmcaptureWizard
        frmCW.ShowDialog()
        frmCW.Dispose()
        frmCW = Nothing
    End Sub

    Private Sub rbInput_CheckedChanged(sender As System.Object, e As System.EventArgs)
        cdCapture.ShowDialog()
    End Sub

    Private Sub btnVideoColorMin_Click(sender As System.Object, e As System.EventArgs) Handles btnVideoColorMin.Click
        cdCapture.Color = btnVideoColorMin.BackColor
        cdCapture.ShowDialog()
        If cdCapture.Color = btnVideoColorMin.BackColor Then Exit Sub
        Dim curMin As Color = cdCapture.Color
        Dim curMax As Color = btnVideoColorMax.BackColor
        btnVideoColorMin.BackColor = Color.FromArgb(Math.Min(curMin.R, curMax.R), Math.Min(curMin.G, curMax.G), Math.Min(curMin.B, curMax.B))
        btnVideoColorMax.BackColor = Color.FromArgb(Math.Max(curMin.R, curMax.R), Math.Max(curMin.G, curMax.G), Math.Max(curMin.B, curMax.B))
    End Sub

    Private Sub btnVideoColorMax_Click(sender As System.Object, e As System.EventArgs) Handles btnVideoColorMax.Click
        cdCapture.Color = btnVideoColorMax.BackColor
        cdCapture.ShowDialog()
        If cdCapture.Color = btnVideoColorMax.BackColor Then Exit Sub
        Dim curMin As Color = btnVideoColorMin.BackColor
        Dim curMax As Color = cdCapture.Color
        btnVideoColorMin.BackColor = Color.FromArgb(Math.Min(curMin.R, curMax.R), Math.Min(curMin.G, curMax.G), Math.Min(curMin.B, curMax.B))
        btnVideoColorMax.BackColor = Color.FromArgb(Math.Max(curMin.R, curMax.R), Math.Max(curMin.G, curMax.G), Math.Max(curMin.B, curMax.B))
    End Sub

    Private Sub btnVideoWizard_Click(sender As System.Object, e As System.EventArgs) Handles btnVideoWizard.Click
        Dim vwf As New frmcaptureWizard
        vwf.ShowDialog()
        If vwf.Canceled Then Exit Sub
        txtVideoPixelX.Text = vwf.pos.X
        txtVideoPixelY.Text = vwf.pos.Y
        btnVideoColorMin.BackColor = vwf.minColor
        btnVideoColorMax.BackColor = vwf.maxColor
    End Sub
End Class