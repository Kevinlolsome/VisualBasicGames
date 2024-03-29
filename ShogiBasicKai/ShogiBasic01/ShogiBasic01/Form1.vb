﻿Public Class Form1
    Const WHITE As Integer = 1
    Const BLACK As Integer = -1
    Const USE_AB As Boolean = True
    Const YOMI_DEPTH As Integer = 3
    Const HAND_READ As Boolean = True
    Const DEBUG_LOG As Boolean = False
    Const RETURN_LOG As Boolean = False
    Const DEBUG_TIME As Boolean = False
    Const ROBO_TEBAN As Integer = BLACK
    Const BLANK As Integer = 255
    Const BRANCH_WIDTH As Integer = 800
    Class MoveData
        Public r As Byte
        Public r2 As Byte
        Public src As Byte
        Public dst As Byte
        Public hand As Byte = BLANK
    End Class
    Dim best As MoveData = New MoveData
    Dim modosi As MoveData = New MoveData
    Dim Node(BRANCH_WIDTH * (YOMI_DEPTH + 1)) As MoveData
    Dim NodeCount As Integer
    Dim ArrayCount As Integer
    Dim komaname As Array
    Dim board As Array
    Dim state As Integer
    Dim undo As Integer
    Dim range As Array
    Dim tegomaw As Array
    Dim tegomab As Array
    Dim pop As Integer
    Dim all As Array
    Dim komaundo As Integer
    Dim kihumem As Integer
    Dim narimem As Integer
    Dim robomode As Boolean
    Dim table As Array = {0, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 6, 7, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 6, 7}
    Dim score As Array = {0, 100, 300, 400, 600, 800, 1000, 1100, 65535, 900, 800, 800, 1000, 1200, 1300, 100, 300, 400, 600, 800, 1000, 1100, 65535, 900, 800, 800, 1000, 1200, 1300}
    Private Sub Init() Handles Me.HandleCreated
        komaname = {"", "歩", "香", "桂", "銀", "金", "飛", "角", "王", "と", "杏", "圭", "全", "龍", "馬"}
        all = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81}
        board = {16, 17, 18, 19, 22, 19, 18, 17, 16,
                    0, 21, 0, 0, 0, 0, 0, 20, 0,
                    15, 15, 15, 15, 15, 15, 15, 15, 15,
                    0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0,
                    1, 1, 1, 1, 1, 1, 1, 1, 1,
                    0, 6, 0, 0, 0, 0, 0, 7, 0,
                    2, 3, 4, 5, 8, 5, 4, 3, 2}
        tegomaw = {0, 0, 0, 0, 0, 0, 0, 0}
        tegomab = {0, 0, 0, 0, 0, 0, 0, 0}
        state = 0
        undo = BLANK
        komaundo = BLANK
        kihumem = 0
        narimem = BLANK
        robomode = False
        BoardSet()
        Randomize()
        '        For i = 0 To score.Length - 1 Step 1
        '       score(i) = score(i) + Rnd() * score(i) * 0.1
        '      Next
    End Sub
    Private Sub BoardSet()
        Dim p As Integer
        For p = 1 To 81 Step 1
            UnitSet(p)
        Next
    End Sub
    Private Sub UnitSet(ByVal locale As Integer)
        Dim b As Button
        Dim c As Integer
        b = GetButton(locale)
        c = board(locale - 1)
        If c = 0 Then
            b.Text = ""
            b.BackColor = Color.LightGray
        ElseIf 1 <= c And c <= 14 Then
            b.Text = komaname(c)
            b.BackColor = Color.WhiteSmoke
            b.ForeColor = Color.Black
        Else
            b.Text = komaname(c - 14)
            b.BackColor = Color.DimGray
            b.ForeColor = Color.White
        End If
    End Sub
    Private Function UnitRange(ByVal locate As Integer) As Array
        Dim unit As Integer
        locate = locate - 1
        unit = board(locate)
        UnitRange = all
        If unit = 1 Then
            UnitRange = HuRange(locate, 1)
        End If
        If unit = 2 Then
            UnitRange = KyoRange(locate, 1)
        End If
        If unit = 3 Then
            UnitRange = KeimaRange(locate, 1)
        End If
        If unit = 4 Then
            UnitRange = GinRange(locate, 1)
        End If
        If unit = 5 Or unit = 9 Or unit = 10 Or unit = 11 Or unit = 12 Then
            UnitRange = KinRange(locate, 1)
        End If
        If unit = 6 Then
            UnitRange = HisyaRange(locate, 1, False)
        End If
        If unit = 7 Then
            UnitRange = KakuRange(locate, 1, False)
        End If
        If unit = 8 Then
            UnitRange = OuRange(locate)
        End If
        If unit = 13 Then
            UnitRange = HisyaRange(locate, 1, True)
        End If
        If unit = 14 Then
            UnitRange = KakuRange(locate, 1, True)
        End If
        If unit = 15 Then
            UnitRange = HuRange(locate, -1)
        End If
        If unit = 16 Then
            UnitRange = KyoRange(locate, -1)
        End If
        If unit = 17 Then
            UnitRange = KeimaRange(locate, -1)
        End If
        If unit = 18 Then
            UnitRange = GinRange(locate, -1)
        End If
        If unit = 19 Or unit = 23 Or unit = 24 Or unit = 25 Or unit = 26 Then
            UnitRange = KinRange(locate, -1)
        End If
        If unit = 20 Then
            UnitRange = HisyaRange(locate, -1, False)
        End If
        If unit = 21 Then
            UnitRange = KakuRange(locate, -1, False)
        End If
        If unit = 22 Then
            UnitRange = OuRange(locate)
        End If
        If unit = 27 Then
            UnitRange = HisyaRange(locate, -1, True)
        End If
        If unit = 28 Then
            UnitRange = KakuRange(locate, -1, True)
        End If
    End Function
    Private Function RangeCheck(ByVal locate As Integer) As Boolean
        Dim i As Integer
        For i = 0 To range.Length - 1 Step 1
            If range(i) = locate Then
                RangeCheck = True
                Exit Function
            End If
        Next
        If robomode = False Then
            RangeCheck = False
        Else
            RangeCheck = False
        End If
    End Function
    Private Sub AddRange(ByVal dx As Integer, ByVal dy As Integer, ByVal array As Array, ByVal pos As Integer)
        Dim dist As Integer
        If CheckBoardRange(dx, dy) = True Then
            dist = dx + dy * 9 + 1
            If IsWhite(undo) And IsWhite(dist) Then
                Exit Sub
            End If
            If IsBlack(undo) And IsBlack(dist) Then
                Exit Sub
            End If
            array.SetValue(dist, pos)
        End If
    End Sub
    Private Function HuRange(ByVal locate As Integer, ByVal wb As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        HuRange = {BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        dx = x
        dy = y - 1 * wb
        AddRange(dx, dy, HuRange, 0)
    End Function
    Private Function KyoRange(ByVal locate As Integer, ByVal wb As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        Dim i As Integer
        Dim dist As Integer
        Dim a As Array
        a = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        For i = 1 To 8 Step 1
            dx = x
            dy = y - i * wb
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                a.SetValue(dist, i)
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i)
                    Exit For
                End If
            Else
                Exit For
            End If
        Next
        KyoRange = a
    End Function
    Private Function HisyaRange(ByVal locate As Integer, ByVal wb As Integer, ByVal c As Boolean) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        Dim i As Integer
        Dim dist As Integer
        Dim a As Array
        a = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        For i = 1 To 8 Step 1
            dx = x
            dy = y - i * wb
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                a.SetValue(dist, i)
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i)
                    Exit For
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x
            dy = y + i * wb
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                a.SetValue(dist, i + 7)
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i + 7)
                    Exit For
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x - i * wb
            dy = y
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                a.SetValue(dist, i + 15)
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i + 15)
                    Exit For
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x + i * wb
            dy = y
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                a.SetValue(dist, i + 23)
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i + 23)
                    Exit For
                End If
            Else
                Exit For
            End If
        Next
        If c = True Then
            dx = x - 1
            dy = y - 1
            AddRange(dx, dy, a, 32)
            dx = x + 1
            dy = y + 1
            AddRange(dx, dy, a, 33)
            dx = x - 1
            dy = y + 1
            AddRange(dx, dy, a, 34)
            dx = x + 1
            dy = y - 1
            AddRange(dx, dy, a, 35)
        End If
        HisyaRange = a
    End Function
    Private Function KakuRange(ByVal locate As Integer, ByVal wb As Integer, ByVal c As Boolean) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        Dim i As Integer
        Dim dist As Integer
        Dim a As Array
        a = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK,
                BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        For i = 1 To 8 Step 1
            dx = x - i
            dy = y - i
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i)
                    Exit For
                ElseIf wb = -1 And IsBlack(undo) = IsWhite(dist) Then
                    a.SetValue(dist, i)
                    Exit For
                Else
                    a.SetValue(dist, i)
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x + i
            dy = y + i
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i + 7)
                    Exit For
                Else
                    a.SetValue(dist, i + 7)
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x - i
            dy = y + i
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                If IsWB(wb, undo) = IsWB(-wb, dist) Then
                    a.SetValue(dist, i + 15)
                    Exit For
                Else
                    a.SetValue(dist, i + 15)
                End If
            Else
                Exit For
            End If
        Next
        For i = 1 To 8 Step 1
            dx = x + i
            dy = y - i
            If CheckBoardRange(dx, dy) = True Then
                dist = dx + dy * 9 + 1
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    Exit For
                End If
                If IsWB(wb, undo) = IsWB(wb, dist) Then
                    a.SetValue(dist, i + 23)
                    Exit For
                Else
                    a.SetValue(dist, i + 23)
                End If
            Else
                Exit For
            End If
        Next
        If c = True Then
            dx = x
            dy = y - 1
            AddRange(dx, dy, a, 32)
            dx = x
            dy = y + 1
            AddRange(dx, dy, a, 33)
            dx = x - 1
            dy = y
            AddRange(dx, dy, a, 34)
            dx = x + 1
            dy = y
            AddRange(dx, dy, a, 35)
        End If
        KakuRange = a
    End Function
    Private Function KeimaRange(ByVal locate As Integer, ByVal wb As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        KeimaRange = {BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        dx = x + 1
        dy = y - 2 * wb
        AddRange(dx, dy, KeimaRange, 0)
        dx = x - 1
        dy = y - 2 * wb
        AddRange(dx, dy, KeimaRange, 1)
    End Function
    Private Function GinRange(ByVal locate As Integer, ByVal wb As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        GinRange = {BLANK, BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        dx = x
        dy = y - 1 * wb
        AddRange(dx, dy, GinRange, 0)
        dx = x + 1
        dy = y - 1
        AddRange(dx, dy, GinRange, 1)
        dx = x - 1
        dy = y - 1
        AddRange(dx, dy, GinRange, 2)
        dx = x + 1
        dy = y + 1
        AddRange(dx, dy, GinRange, 3)
        dx = x - 1
        dy = y + 1
        AddRange(dx, dy, GinRange, 4)
    End Function
    Private Function KinRange(ByVal locate As Integer, ByVal wb As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        KinRange = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        dx = x
        dy = y - 1
        AddRange(dx, dy, KinRange, 0)
        dx = x + 1
        dy = y - 1 * wb
        AddRange(dx, dy, KinRange, 1)
        dx = x - 1
        dy = y - 1 * wb
        AddRange(dx, dy, KinRange, 2)
        dx = x + 1
        dy = y
        AddRange(dx, dy, KinRange, 3)
        dx = x - 1
        dy = y
        AddRange(dx, dy, KinRange, 4)
        dx = x
        dy = y + 1
        AddRange(dx, dy, KinRange, 5)
    End Function
    Private Function OuRange(ByVal locate As Integer) As Array
        Dim x As Integer
        Dim y As Integer
        Dim dx As Integer
        Dim dy As Integer
        OuRange = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK}
        x = locate - Int(locate / 9) * 9
        y = Int(locate / 9)
        dx = x
        dy = y - 1
        AddRange(dx, dy, OuRange, 0)
        dx = x + 1
        dy = y - 1
        AddRange(dx, dy, OuRange, 1)
        dx = x - 1
        dy = y - 1
        AddRange(dx, dy, OuRange, 2)
        dx = x + 1
        dy = y + 1
        AddRange(dx, dy, OuRange, 3)
        dx = x - 1
        dy = y + 1
        AddRange(dx, dy, OuRange, 4)
        dx = x + 1
        dy = y
        AddRange(dx, dy, OuRange, 5)
        dx = x - 1
        dy = y
        AddRange(dx, dy, OuRange, 6)
        dx = x
        dy = y + 1
        AddRange(dx, dy, OuRange, 7)
    End Function
    Private Function CheckBoardRange(ByVal x As Integer, ByVal y As Integer) As Boolean
        CheckBoardRange = True
        If x < 0 Or 8 < x Then
            CheckBoardRange = False
        End If
        If y < 0 Or 8 < y Then
            CheckBoardRange = False
        End If
    End Function
    Private Function IsWhite(ByVal locate As Integer) As Boolean
        Dim c As Integer
        IsWhite = False
        If locate < 1 Or 81 < locate Then
            Exit Function
        End If
        c = board(locate - 1)
        If 1 <= c And c <= 14 Then
            IsWhite = True
        End If
    End Function
    Private Function IsBlack(ByVal locate As Integer) As Boolean
        Dim c As Integer
        IsBlack = False
        If locate < 1 Or 81 < locate Then
            Exit Function
        End If
        c = board(locate - 1)
        If 15 <= c And c <= 30 Then
            IsBlack = True
        End If
    End Function
    Private Function IsWB(ByVal wb As Integer, ByVal i As Integer) As Boolean
        If wb = WHITE Then
            IsWB = IsWhite(i)
        Else
            IsWB = IsBlack(i)
        End If
    End Function
    Private Function max(ByVal a As Integer, ByVal b As Integer)
        If a >= b Then
            max = a
        Else
            max = b
        End If
        Return max
    End Function
    Private Function min(ByVal a As Integer, ByVal b As Integer)
        If a <= b Then
            min = a
        Else
            min = b
        End If
        Return min
    End Function
    Function ReverceWB(ByVal wb As Integer) As Integer
        If wb = -1 Then
            ReverceWB = 1
        Else
            ReverceWB = -1
        End If
    End Function
    Private Function KomaScore(ByVal koma) As Integer
        KomaScore = score(koma)
    End Function
    Private Function Hyouka() As Integer
        Dim arw As Array = {1, 2, 3, 4, 5, 6, 7, 8}
        Dim arb As Array = {15, 16, 17, 18, 19, 20, 21, 22}
        Hyouka = 0
        For i = 0 To 80 Step 1
            If IsWB(WHITE, i + 1) Then
                Hyouka += KomaScore(board(i))
            End If
            If IsWB(BLACK, i + 1) Then
                Hyouka -= KomaScore(board(i))
            End If
        Next
        For i = 0 To 7 Step 1
            Hyouka += tegomaw(i) * KomaScore(arw(i)) * 1.05
            Hyouka -= tegomab(i) * KomaScore(arb(i)) * 1.05
        Next
        Hyouka = Hyouka / 2
        Return Hyouka
    End Function
    Private Function alphabeta(ByVal first As Integer, ByVal wb As Integer, ByVal depth As Integer,
                            ByVal alpha As Integer, ByVal beta As Integer) As Integer
        If depth = 0 Then
            Return Hyouka() * wb
        End If
        Dim last As Integer = GenerateMoves(first, wb)
        For i = first To last - 1
            MakeMove(Node(i), False)
            Dim a = -alphabeta(last, -wb, depth - 1, -beta, -alpha)
            UnmakeMove(Node(i))
            If (a > alpha) Then
                alpha = a
                If depth = YOMI_DEPTH Then
                    best = Node(i)
                End If
            End If
            If alpha >= beta Then
                Return alpha
            End If
        Next
        Return alpha
    End Function
    Private Function GenerateMoves(ByVal first As Integer, ByVal wb As Integer) As Integer
        Dim idx As Integer = first
        If HAND_READ Then
            For i = 0 To 7
                If wb = 1 Then
                    If tegomaw(i) > 0 Then
                        range = HandRange(wb, i)
                        For j = 0 To range.Length - 1 Step 1
                            If range(j) <> BLANK Then
                                Node(idx) = New MoveData
                                Node(idx).hand = i
                                Node(idx).r2 = range(j)
                                NodeCount += 1
                                idx += 1
                            End If
                        Next
                    End If
                Else
                    If tegomab(i) > 0 Then
                        range = HandRange(wb, i)
                        For j = 0 To range.Length - 1 Step 1
                            If range(j) <> BLANK Then
                                Node(idx) = New MoveData
                                Node(idx).hand = i + 15
                                Node(idx).r2 = range(j)
                                NodeCount += 1
                                idx += 1
                            End If
                        Next
                    End If
                End If
            Next
        End If
        For i = 1 To 81 Step 1
            If True = IsWB(wb, i) Then
                undo = i
                range = UnitRange(i)
                ArrayCount += range.Length
                For j = 0 To range.Length - 1 Step 1
                    If range(j) <> BLANK Then
                        Node(idx) = New MoveData
                        Node(idx).r = i
                        Node(idx).r2 = range(j)
                        Node(idx).src = board(i - 1)
                        Node(idx).dst = board(range(j) - 1)
                        Node(idx).hand = BLANK
                        NodeCount += 1
                        idx += 1
                    End If
                Next
            End If
        Next
        Return idx
    End Function
    Private Sub RobotMove(ByVal wb As Integer)
        Dim c As Integer
        Dim r As Integer
        Dim r2 As Integer
        Dim undobuf As Integer
        Dim PutRetire As Boolean = False
        Dim nodemax As Integer
        Dim nodemin As Integer
        c = 0
        NodeCount = 0
        ArrayCount = 0
        Dim starttime As Long = Now.Hour * 3600 + Now.Minute * 60 + Now.Second
        undobuf = undo
        nodemax = 214748364
        nodemin = -214748364
        Dim MakeBuff As MoveData = New MoveData
        Dim FindFlag As Boolean = False
        SuspendLayout()
        best.r = BLANK
        Dim ret As Integer = alphabeta(0, wb, YOMI_DEPTH, nodemin, nodemax)
        If RETURN_LOG Then
            ListBox1.Items.Add(ret)
        End If
        robomode = True
        ResumeLayout()
        If best.r = BLANK Then
            ListBox1.Items.Add("▽投了")
            ListBox1.TopIndex = ListBox1.Items.Count - 1
        ElseIf best.hand = BLANK Then
            r = best.r
            r2 = best.r2
            GetButton(r).PerformClick()
            GetButton(r2).PerformClick()
            robomode = False
        Else
            r = best.hand - 14
            r2 = best.r2
            GetHandBlack(r).PerformClick()
            GetButton(r2).PerformClick()
            robomode = False
        End If
    End Sub
    Private Sub UnitClick(ByVal locate As Integer)
        Dim b As Button
        Dim c As Integer
        Dim r As Integer
        r = False
        If state = 0 Then
            undo = locate
            range = UnitRange(locate)
            For i = 0 To range.Length - 1 Step 1
                If range(i) <> BLANK Then
                    r = True
                End If
            Next
            If r = False Then
                Exit Sub
            End If
            If IsWhite(locate) = True Then
                state = 1
            ElseIf IsBlack(locate) = True Then
                state = 2
            Else
                Exit Sub
            End If
            For c = 0 To range.Length - 1 Step 1
                b = GetButton(range(c))
                If IsWhite(locate) = True Then
                    b.BackColor = Color.RoyalBlue
                Else
                    b.BackColor = Color.OrangeRed
                End If
            Next
        ElseIf state = 1 And RangeCheck(locate) Then
            'MoveChara(locate)
            Dim d As MoveData = New MoveData
            d.hand = BLANK
            d.r = undo
            d.r2 = locate
            d.src = board(undo - 1)
            d.dst = board(locate - 1)
            MakeMove(d, True)
            DispAll()
            AddKihu(locate)
            state = 0
            Me.Refresh()
            Me.Cursor = Cursors.WaitCursor
            RobotMove(-1)
            Me.Cursor = Cursors.Default
        ElseIf (state = 1 Or state = 2) And undo = locate Then
            DispAll()
            state = 0
        ElseIf state = 2 And RangeCheck(locate) Then
            'MoveChara(locate)
            Dim d As MoveData = New MoveData
            d.hand = BLANK
            d.r = undo
            d.r2 = locate
            d.src = board(undo - 1)
            d.dst = board(locate - 1)
            MakeMove(d, True)
            DispAll()
            AddKihu(locate)
            state = 0
        ElseIf state = 3 And RangeCheck(locate) Then
            'board(locate - 1) = pop
            'tegomaw(pop - 1) = tegomaw(pop - 1) - 1
            Dim d As MoveData = New MoveData
            d.hand = pop
            d.r = undo
            d.r2 = locate
            d.src = board(undo - 1)
            d.dst = board(locate - 1)
            MakeMove(d, True)
            DispAll()
            AddKihu(locate)
            undo = BLANK
            komaundo = BLANK
            state = 0
            Me.Refresh()
            Me.Cursor = Cursors.WaitCursor
            RobotMove(-1)
            Me.Cursor = Cursors.Default
        ElseIf state = 4 And RangeCheck(locate) Then
            'board(locate - 1) = pop
            'tegomab(pop - 15) = tegomab(pop - 15) - 1
            Dim d As MoveData = New MoveData
            d.hand = pop
            d.r = undo
            d.r2 = locate
            d.src = board(undo - 1)
            d.dst = board(locate - 1)
            MakeMove(d, True)
            DispAll()
            AddKihu(locate)
            undo = BLANK
            komaundo = BLANK
            state = 0
        End If
    End Sub
    Private Sub DispAll()
        For p = 1 To 81 Step 1
            UnitSet(p)
        Next
        DispHand()
        TextBox3.Text = Hyouka().ToString
        TextBox4.Text = -Hyouka().ToString
    End Sub
    Private Function HandRange(ByVal wb As Integer, ByVal idx As Integer) As Array
        Dim arw As Array = {1, 2, 3, 4, 5, 6, 7, 8}
        Dim arb As Array = {15, 16, 17, 18, 19, 20, 21, 22}
        Dim koma = arb(idx)
        If wb = 1 Then
            koma = arw(idx)
        End If
        range = all.Clone()
        For i = 0 To 80 Step 1
            If board(i) <> 0 Then
                range(i) = BLANK
            End If
        Next
        If koma = 1 Then
            For i = 0 To 8 Step 1
                range(i) = BLANK
            Next
            For i = 0 To 8 Step 1
                For j = 0 To 8
                    If board(i + j * 9) = 1 Then
                        For k = 0 To 8
                            range(i + k * 9) = BLANK
                        Next
                    End If
                Next
            Next
        End If
        If koma = 2 Then
            For i = 0 To 8 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 3 Then
            For i = 0 To 17 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 15 Then
            For i = 72 To 80 Step 1
                range(i) = BLANK
            Next
            For i = 0 To 8 Step 1
                For j = 0 To 8
                    If board(i + j * 9) = 15 Then
                        For k = 0 To 8
                            range(i + k * 9) = BLANK
                        Next
                    End If
                Next
            Next
        End If
        If koma = 16 Then
            For i = 72 To 80 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 17 Then
            For i = 63 To 80 Step 1
                range(i) = BLANK
            Next
        End If
        HandRange = range.Clone
    End Function
    Private Function TakeHand(ByVal koma As Integer) As Array
        Dim b As Button
        pop = koma
        range = all.Clone()
        For i = 0 To 80 Step 1
            If board(i) <> 0 Then
                range(i) = BLANK
            End If
        Next
        If koma = 1 Then
            For i = 0 To 8 Step 1
                range(i) = BLANK
            Next
            For i = 0 To 8 Step 1
                For j = 0 To 8
                    If board(i + j * 9) = 1 Then
                        For k = 0 To 8
                            range(i + k * 9) = BLANK
                        Next
                    End If
                Next
            Next
        End If
        If koma = 2 Then
            For i = 0 To 8 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 3 Then
            For i = 0 To 17 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 15 Then
            For i = 72 To 80 Step 1
                range(i) = BLANK
            Next
            For i = 0 To 8 Step 1
                For j = 0 To 8
                    If board(i + j * 9) = 15 Then
                        For k = 0 To 8
                            range(i + k * 9) = BLANK
                        Next
                    End If
                Next
            Next
        End If
        If koma = 16 Then
            For i = 72 To 80 Step 1
                range(i) = BLANK
            Next
        End If
        If koma = 17 Then
            For i = 63 To 80 Step 1
                range(i) = BLANK
            Next
        End If
        If komaundo <> BLANK Then
            range = {}
            DispAll()
            komaundo = BLANK
            state = 0
            TakeHand = range.Clone
            Exit Function
        Else
            komaundo = koma
        End If
        If 1 <= koma Or koma <= 14 Then
            state = 3
            For c = 0 To range.Length - 1 Step 1
                b = GetButton(range(c))
                b.BackColor = Color.RoyalBlue
            Next
        End If
        If 15 <= koma Then
            state = 4
            For c = 0 To range.Length - 1 Step 1
                b = GetButton(range(c))
                b.BackColor = Color.OrangeRed
            Next
        End If
        TakeHand = range.Clone
    End Function
    Private Sub DispHand()
        Dim i As Integer
        For i = 1 To tegomaw.Length Step 1
            GetHandWhite(i).Text = GetKomaName(i) + Str(tegomaw(i - 1))
            If tegomaw(i - 1) > 0 Then
                GetHandWhite(i).Visible = True
            Else
                GetHandWhite(i).Visible = False
            End If
        Next
        For i = 1 To tegomab.Length Step 1
            GetHandBlack(i).Text = GetKomaName(i) + Str(tegomab(i - 1))
            If tegomab(i - 1) > 0 Then
                GetHandBlack(i).Visible = True
            Else
                GetHandBlack(i).Visible = False
            End If
        Next
    End Sub
    Private Sub AddYomi(ByVal locate As Integer)
        Dim x As Integer
        Dim y As Integer
        Dim koma As String
        Dim uchi As String
        Dim triangle As String
        Dim pos As String
        Dim nari As String
        Dim wb As Integer
        If IsWhite(locate) = True Then
            wb = 1
            triangle = "▲"
        Else
            wb = -1
            triangle = "▽"
        End If
        locate = locate - 1
        x = locate - Int(locate / 9) * 9 + 1
        y = Int(locate / 9) + 1
        If komaundo <> BLANK Then
            uchi = "打"
        Else
            uchi = ""
        End If
        If locate = kihumem Then
            pos = " 同"
        Else
            pos = Str(x) + GetKanji(y)
        End If
        If narimem <> BLANK And narimem <> board(locate) Then
            koma = GetKomaName(narimem)
            nari = "成"
        Else
            koma = GetKomaName(board(locate))
            nari = ""
        End If
        ListBox1.Items.Add("Y" + triangle + pos + koma + GetSoeji(locate + 1) + uchi + nari)
        ListBox1.Items.Add(Hyouka() * ROBO_TEBAN * wb)
        ListBox1.TopIndex = ListBox1.Items.Count - 1
        kihumem = locate
        narimem = BLANK
    End Sub
    Private Sub MakeMove(ByVal d As MoveData, ByVal mov As Boolean)
        modosi = d
        If d.hand <> BLANK Then
            KomaOki(d.r2, d.hand)
            GoTo LOG_WRITE
        End If
        KomaTori(d.r2)
        If mov Then
            narimem = board(d.r - 1)
        End If
        ClassUp(d.r)
        board(d.r2 - 1) = board(d.r - 1)
        board(d.r - 1) = 0
        ClassUp(d.r2)
LOG_WRITE:
        If DEBUG_LOG Then
            AddYomi(d.r2)
        End If
    End Sub
    Private Sub UnmakeMove(ByVal d As MoveData)
        If d.hand <> BLANK Then
            KomaModosi(d.r2)
            board(d.r2 - 1) = 0
            Exit Sub
        End If
        board(d.r - 1) = d.src
        KomaKaeshi(d.r2, d.dst)
    End Sub
    Private Function Question() As Boolean
        Question = True
        If My.Computer.Keyboard.ShiftKeyDown Then
            Question = False
        End If
        If My.Computer.Keyboard.CtrlKeyDown Then
            Question = False
        End If
    End Function
    Private Sub ClassUp(ByVal locate As Integer)
        Dim unit As Integer
        unit = board(locate - 1)
        If IsWhite(locate) And 1 <= locate And locate <= 27 Then
            If Question() = False Then
                Exit Sub
            End If
            If unit = 1 Then
                board(locate - 1) = 9
            End If
            If unit = 2 Then
                board(locate - 1) = 10
            End If
            If unit = 3 Then
                board(locate - 1) = 11
            End If
            If unit = 4 Then
                board(locate - 1) = 12
            End If
            If unit = 6 Then
                board(locate - 1) = 13
            End If
            If unit = 7 Then
                board(locate - 1) = 14
            End If
        End If
        If IsBlack(locate) And 55 <= locate And locate <= 81 Then
            If unit = 15 Then
                board(locate - 1) = 23
            End If
            If unit = 16 Then
                board(locate - 1) = 24
            End If
            If unit = 17 Then
                board(locate - 1) = 25
            End If
            If unit = 18 Then
                board(locate - 1) = 26
            End If
            If unit = 20 Then
                board(locate - 1) = 27
            End If
            If unit = 21 Then
                board(locate - 1) = 28
            End If
        End If
    End Sub
    Private Sub KomaTori(ByVal locate)
        Dim t As Integer
        t = board(locate - 1)
        If 15 <= t Then
            t = table(t) - 1
            tegomaw(t) = tegomaw(t) + 1
        ElseIf 1 <= t And t <= 14 Then
            t = table(t) - 1
            tegomab(t) = tegomab(t) + 1
        End If
    End Sub
    Private Sub KomaModosi(ByVal locate)
        Dim t As Integer
        t = board(locate - 1)
        If 15 <= t Then
            t = table(t) - 1
            tegomab(t) = tegomab(t) + 1
        ElseIf 1 <= t And t <= 14 Then
            t = table(t) - 1
            tegomaw(t) = tegomaw(t) + 1
        End If
    End Sub
    Private Sub KomaOki(ByVal locate, ByVal t)
        board(locate - 1) = t
        If 15 <= t Then
            t = table(t) - 1
            tegomab(t) = tegomab(t) - 1
        ElseIf 1 <= t And t <= 14 Then
            t = table(t) - 1
            tegomaw(t) = tegomaw(t) - 1
        End If
    End Sub
    Private Sub KomaKaeshi(ByVal locate, ByVal t)
        board(locate - 1) = t
        If 15 <= t Then
            t = table(t) - 1
            tegomaw(t) = tegomaw(t) - 1
        ElseIf 1 <= t And t <= 14 Then
            t = table(t) - 1
            tegomab(t) = tegomab(t) - 1
        End If
    End Sub
    Private Sub AddKihu(ByVal locate As Integer)
        Dim x As Integer
        Dim y As Integer
        Dim koma As String
        Dim uchi As String
        Dim triangle As String
        Dim pos As String
        Dim nari As String
        If IsWhite(locate) = True Then
            triangle = "▲"
        Else
            triangle = "▽"
        End If
        locate = locate - 1
        x = locate - Int(locate / 9) * 9 + 1
        y = Int(locate / 9) + 1
        If komaundo <> BLANK Then
            uchi = "打"
        Else
            uchi = ""
        End If
        If locate = kihumem Then
            pos = " 同"
        Else
            pos = Str(x) + GetKanji(y)
        End If
        If narimem <> BLANK And narimem <> board(locate) Then
            koma = GetKomaName(narimem)
            nari = "成"
        Else
            koma = GetKomaName(board(locate))
            nari = ""
        End If
        ListBox1.Items.Add(triangle + pos + koma + GetSoeji(locate + 1) + uchi + nari)
        ListBox1.TopIndex = ListBox1.Items.Count - 1
        kihumem = locate
        narimem = BLANK
    End Sub
    Private Function GetKomaName(ByVal index As Integer) As String
        If 15 <= index Then
            index = index - 14
        End If
        GetKomaName = komaname(index)
    End Function
    Private Function GetKanji(ByVal num As Integer) As String
        Dim k As Array
        k = {"零", "一", "二", "三", "四", "五", "六", "七", "八", "九"}
        GetKanji = k(num)
    End Function
    Private Function GetSoeji(ByVal locate As Integer) As String
        Dim prev As Array
        Dim Range As Array
        Dim Kouho As Array
        Dim ki As Integer
        Dim x As Integer
        Dim y As Integer
        Dim ubuf As Integer
        Dim lx As Integer
        Dim ly As Integer
        Dim lbuf As Integer
        Dim dx As Integer
        Dim dy As Integer
        Dim xc As Integer
        Dim yc As Integer
        Dim dl As Integer
        Dim lr As String
        Dim ud As String
        Kouho = {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK}
        ki = 0
        lr = ""
        ud = ""
        prev = board.Clone
        If undo = BLANK Then
            GetSoeji = ""
            Exit Function
        End If
        prev(undo - 1) = board(locate - 1)
        prev(locate - 1) = 0
        For i = 1 To 81 Step 1
            If i = undo Then
            ElseIf prev(i - 1) = prev(undo - 1) Then
                Range = UnitRange(i)
                For j = 0 To Range.Length - 1 Step 1
                    If Range(j) = locate Then
                        Kouho(ki) = i
                    End If
                Next
            End If
        Next
        lbuf = locate - 1
        lx = lbuf - Int(lbuf / 9) * 9 + 1
        ly = Int(lbuf / 9) + 1
        ubuf = undo - 1
        x = ubuf - Int(ubuf / 9) * 9 + 1
        y = Int(ubuf / 9) + 1
        xc = 0
        yc = 0
        For i = 0 To Kouho.Length - 1 Step 1
            If Kouho(i) <> BLANK Then
                dl = Kouho(i)
                dl = dl - 1
                dx = dl - Int(dl / 9) * 9 + 1
                dy = Int(dl / 9) + 1
                If x = dx Then
                    xc = xc + 1
                End If
                If y = dy Then
                    yc = yc + 1
                End If
            End If
        Next

        For i = 0 To Kouho.Length - 1 Step 1
            If Kouho(i) <> BLANK Then
                dl = Kouho(i)
                dl = dl - 1
                dx = dl - Int(dl / 9) * 9 + 1
                dy = Int(dl / 9) + 1
                If 1 <= yc Then
                    If state = 1 Then
                        If x < dx Then
                            lr = "右"
                        ElseIf dx < x Then
                            lr = "左"
                        End If
                    ElseIf state = 2 Then
                        If x < dx Then
                            lr = "左"
                        ElseIf dx < x Then
                            lr = "右"
                        End If
                    End If
                End If
                If 0 = xc Then
                    If state = 1 Then
                        If y < dy Then
                            ud = "引"
                        ElseIf dy < y Then
                            ud = "上"
                        End If
                    ElseIf state = 2 Then
                        If y < dy Then
                            ud = "上"
                        ElseIf dy < y Then
                            ud = "引"
                        End If
                    End If
                End If
            End If
        Next
        GetSoeji = lr + ud
    End Function
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        UnitClick(1)
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        UnitClick(2)
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        UnitClick(3)
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        UnitClick(4)
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        UnitClick(5)
    End Sub
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        UnitClick(6)
    End Sub
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        UnitClick(7)
    End Sub
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        UnitClick(8)
    End Sub
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        UnitClick(9)
    End Sub
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        UnitClick(10)
    End Sub
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        UnitClick(11)
    End Sub
    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        UnitClick(12)
    End Sub
    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        UnitClick(13)
    End Sub
    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        UnitClick(14)
    End Sub
    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        UnitClick(15)
    End Sub
    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        UnitClick(16)
    End Sub
    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        UnitClick(17)
    End Sub
    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        UnitClick(18)
    End Sub
    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        UnitClick(19)
    End Sub
    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        UnitClick(20)
    End Sub
    Private Sub Button21_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click
        UnitClick(21)
    End Sub
    Private Sub Button22_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button22.Click
        UnitClick(22)
    End Sub
    Private Sub Button23_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button23.Click
        UnitClick(23)
    End Sub
    Private Sub Button24_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        UnitClick(24)
    End Sub
    Private Sub Button25_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button25.Click
        UnitClick(25)
    End Sub
    Private Sub Button26_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button26.Click
        UnitClick(26)
    End Sub
    Private Sub Button27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click
        UnitClick(27)
    End Sub
    Private Sub Button28_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button28.Click
        UnitClick(28)
    End Sub
    Private Sub Button29_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button29.Click
        UnitClick(29)
    End Sub
    Private Sub Button30_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button30.Click
        UnitClick(30)
    End Sub
    Private Sub Button31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button31.Click
        UnitClick(31)
    End Sub
    Private Sub Button32_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button32.Click
        UnitClick(32)
    End Sub
    Private Sub Button33_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button33.Click
        UnitClick(33)
    End Sub
    Private Sub Button34_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button34.Click
        UnitClick(34)
    End Sub
    Private Sub Button35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button35.Click
        UnitClick(35)
    End Sub
    Private Sub Button36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button36.Click
        UnitClick(36)
    End Sub
    Private Sub Button37_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button37.Click
        UnitClick(37)
    End Sub
    Private Sub Button38_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button38.Click
        UnitClick(38)
    End Sub
    Private Sub Button39_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button39.Click
        UnitClick(39)
    End Sub
    Private Sub Button40_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button40.Click
        UnitClick(40)
    End Sub
    Private Sub Button41_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button41.Click
        UnitClick(41)
    End Sub
    Private Sub Button42_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button42.Click
        UnitClick(42)
    End Sub
    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click
        UnitClick(43)
    End Sub
    Private Sub Button44_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button44.Click
        UnitClick(44)
    End Sub
    Private Sub Button45_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button45.Click
        UnitClick(45)
    End Sub
    Private Sub Button46_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button46.Click
        UnitClick(46)
    End Sub
    Private Sub Button47_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button47.Click
        UnitClick(47)
    End Sub
    Private Sub Button48_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button48.Click
        UnitClick(48)
    End Sub
    Private Sub Button49_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button49.Click
        UnitClick(49)
    End Sub
    Private Sub Button50_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button50.Click
        UnitClick(50)
    End Sub
    Private Sub Button51_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button51.Click
        UnitClick(51)
    End Sub
    Private Sub Button52_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button52.Click
        UnitClick(52)
    End Sub
    Private Sub Button53_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button53.Click
        UnitClick(53)
    End Sub
    Private Sub Button54_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button54.Click
        UnitClick(54)
    End Sub
    Private Sub Button55_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button55.Click
        UnitClick(55)
    End Sub
    Private Sub Button56_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button56.Click
        UnitClick(56)
    End Sub
    Private Sub Button57_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button57.Click
        UnitClick(57)
    End Sub
    Private Sub Button58_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button58.Click
        UnitClick(58)
    End Sub
    Private Sub Button59_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button59.Click
        UnitClick(59)
    End Sub
    Private Sub Button60_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button60.Click
        UnitClick(60)
    End Sub
    Private Sub Button61_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button61.Click
        UnitClick(61)
    End Sub
    Private Sub Button62_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button62.Click
        UnitClick(62)
    End Sub
    Private Sub Button63_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button63.Click
        UnitClick(63)
    End Sub
    Private Sub Button64_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button64.Click
        UnitClick(64)
    End Sub
    Private Sub Button65_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button65.Click
        UnitClick(65)
    End Sub
    Private Sub Button66_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button66.Click
        UnitClick(66)
    End Sub
    Private Sub Button67_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button67.Click
        UnitClick(67)
    End Sub
    Private Sub Button68_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button68.Click
        UnitClick(68)
    End Sub
    Private Sub Button69_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button69.Click
        UnitClick(69)
    End Sub
    Private Sub Button70_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button70.Click
        UnitClick(70)
    End Sub
    Private Sub Button71_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button71.Click
        UnitClick(71)
    End Sub
    Private Sub Button72_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button72.Click
        UnitClick(72)
    End Sub
    Private Sub Button73_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button73.Click
        UnitClick(73)
    End Sub
    Private Sub Button74_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button74.Click
        UnitClick(74)
    End Sub
    Private Sub Button75_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button75.Click
        UnitClick(75)
    End Sub
    Private Sub Button76_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button76.Click
        UnitClick(76)
    End Sub
    Private Sub Button77_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button77.Click
        UnitClick(77)
    End Sub
    Private Sub Button78_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button78.Click
        UnitClick(78)
    End Sub
    Private Sub Button79_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button79.Click
        UnitClick(79)
    End Sub
    Private Sub Button80_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button80.Click
        UnitClick(80)
    End Sub
    Private Sub Button81_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button81.Click
        UnitClick(81)
    End Sub
    Private Function GetButton(ByVal locate As Integer) As Button
        GetButton = Button255
        If locate = 1 Then
            GetButton = Button1
        End If
        If locate = 2 Then
            GetButton = Button2
        End If
        If locate = 3 Then
            GetButton = Button3
        End If
        If locate = 4 Then
            GetButton = Button4
        End If
        If locate = 5 Then
            GetButton = Button5
        End If
        If locate = 6 Then
            GetButton = Button6
        End If
        If locate = 7 Then
            GetButton = Button7
        End If
        If locate = 8 Then
            GetButton = Button8
        End If
        If locate = 9 Then
            GetButton = Button9
        End If
        If locate = 10 Then
            GetButton = Button10
        End If
        If locate = 11 Then
            GetButton = Button11
        End If
        If locate = 12 Then
            GetButton = Button12
        End If
        If locate = 13 Then
            GetButton = Button13
        End If
        If locate = 14 Then
            GetButton = Button14
        End If
        If locate = 15 Then
            GetButton = Button15
        End If
        If locate = 16 Then
            GetButton = Button16
        End If
        If locate = 17 Then
            GetButton = Button17
        End If
        If locate = 18 Then
            GetButton = Button18
        End If
        If locate = 19 Then
            GetButton = Button19
        End If
        If locate = 20 Then
            GetButton = Button20
        End If
        If locate = 21 Then
            GetButton = Button21
        End If
        If locate = 22 Then
            GetButton = Button22
        End If
        If locate = 23 Then
            GetButton = Button23
        End If
        If locate = 24 Then
            GetButton = Button24
        End If
        If locate = 25 Then
            GetButton = Button25
        End If
        If locate = 26 Then
            GetButton = Button26
        End If
        If locate = 27 Then
            GetButton = Button27
        End If
        If locate = 28 Then
            GetButton = Button28
        End If
        If locate = 29 Then
            GetButton = Button29
        End If
        If locate = 30 Then
            GetButton = Button30
        End If
        If locate = 31 Then
            GetButton = Button31
        End If
        If locate = 32 Then
            GetButton = Button32
        End If
        If locate = 33 Then
            GetButton = Button33
        End If
        If locate = 34 Then
            GetButton = Button34
        End If
        If locate = 35 Then
            GetButton = Button35
        End If
        If locate = 36 Then
            GetButton = Button36
        End If
        If locate = 37 Then
            GetButton = Button37
        End If
        If locate = 38 Then
            GetButton = Button38
        End If
        If locate = 39 Then
            GetButton = Button39
        End If
        If locate = 40 Then
            GetButton = Button40
        End If
        If locate = 41 Then
            GetButton = Button41
        End If
        If locate = 42 Then
            GetButton = Button42
        End If
        If locate = 43 Then
            GetButton = Button43
        End If
        If locate = 44 Then
            GetButton = Button44
        End If
        If locate = 45 Then
            GetButton = Button45
        End If
        If locate = 46 Then
            GetButton = Button46
        End If
        If locate = 47 Then
            GetButton = Button47
        End If
        If locate = 48 Then
            GetButton = Button48
        End If
        If locate = 49 Then
            GetButton = Button49
        End If
        If locate = 50 Then
            GetButton = Button50
        End If
        If locate = 51 Then
            GetButton = Button51
        End If
        If locate = 52 Then
            GetButton = Button52
        End If
        If locate = 53 Then
            GetButton = Button53
        End If
        If locate = 54 Then
            GetButton = Button54
        End If
        If locate = 55 Then
            GetButton = Button55
        End If
        If locate = 56 Then
            GetButton = Button56
        End If
        If locate = 57 Then
            GetButton = Button57
        End If
        If locate = 58 Then
            GetButton = Button58
        End If
        If locate = 59 Then
            GetButton = Button59
        End If
        If locate = 60 Then
            GetButton = Button60
        End If
        If locate = 61 Then
            GetButton = Button61
        End If
        If locate = 62 Then
            GetButton = Button62
        End If
        If locate = 63 Then
            GetButton = Button63
        End If
        If locate = 64 Then
            GetButton = Button64
        End If
        If locate = 65 Then
            GetButton = Button65
        End If
        If locate = 66 Then
            GetButton = Button66
        End If
        If locate = 67 Then
            GetButton = Button67
        End If
        If locate = 68 Then
            GetButton = Button68
        End If
        If locate = 69 Then
            GetButton = Button69
        End If
        If locate = 70 Then
            GetButton = Button70
        End If
        If locate = 71 Then
            GetButton = Button71
        End If
        If locate = 72 Then
            GetButton = Button72
        End If
        If locate = 73 Then
            GetButton = Button73
        End If
        If locate = 74 Then
            GetButton = Button74
        End If
        If locate = 75 Then
            GetButton = Button75
        End If
        If locate = 76 Then
            GetButton = Button76
        End If
        If locate = 77 Then
            GetButton = Button77
        End If
        If locate = 78 Then
            GetButton = Button78
        End If
        If locate = 79 Then
            GetButton = Button79
        End If
        If locate = 80 Then
            GetButton = Button80
        End If
        If locate = 81 Then
            GetButton = Button81
        End If
    End Function
    Private Function GetHandWhite(ByVal locate As Integer) As Button
        GetHandWhite = Button255
        If locate = 1 Then
            GetHandWhite = ButtonA1
        End If
        If locate = 2 Then
            GetHandWhite = ButtonA2
        End If
        If locate = 3 Then
            GetHandWhite = ButtonA3
        End If
        If locate = 4 Then
            GetHandWhite = ButtonA4
        End If
        If locate = 5 Then
            GetHandWhite = ButtonA5
        End If
        If locate = 6 Then
            GetHandWhite = ButtonA6
        End If
        If locate = 7 Then
            GetHandWhite = ButtonA7
        End If
        If locate = 8 Then
            GetHandWhite = ButtonA8
        End If
    End Function
    Private Function GetHandBlack(ByVal locate As Integer) As Button
        GetHandBlack = Button255
        If locate = 1 Then
            GetHandBlack = ButtonB1
        End If
        If locate = 2 Then
            GetHandBlack = ButtonB2
        End If
        If locate = 3 Then
            GetHandBlack = ButtonB3
        End If
        If locate = 4 Then
            GetHandBlack = ButtonB4
        End If
        If locate = 5 Then
            GetHandBlack = ButtonB5
        End If
        If locate = 6 Then
            GetHandBlack = ButtonB6
        End If
        If locate = 7 Then
            GetHandBlack = ButtonB7
        End If
        If locate = 8 Then
            GetHandBlack = ButtonB8
        End If
    End Function
    Private Sub ButtonA1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA1.Click
        TakeHand(1)
    End Sub
    Private Sub ButtonA2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA2.Click
        TakeHand(2)
    End Sub
    Private Sub ButtonA3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA3.Click
        TakeHand(3)
    End Sub
    Private Sub ButtonA4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA4.Click
        TakeHand(4)
    End Sub
    Private Sub ButtonA5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA5.Click
        TakeHand(5)
    End Sub
    Private Sub ButtonA6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA6.Click
        TakeHand(6)
    End Sub
    Private Sub ButtonA7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA7.Click
        TakeHand(7)
    End Sub
    Private Sub ButtonA8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA8.Click
        TakeHand(8)
    End Sub
    Private Sub ButtonB1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB1.Click
        TakeHand(15)
    End Sub
    Private Sub ButtonB2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB2.Click
        TakeHand(16)
    End Sub
    Private Sub ButtonB3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB3.Click
        TakeHand(17)
    End Sub
    Private Sub ButtonB4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB4.Click
        TakeHand(18)
    End Sub
    Private Sub ButtonB5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB5.Click
        TakeHand(19)
    End Sub
    Private Sub ButtonB6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB6.Click
        TakeHand(20)
    End Sub
    Private Sub ButtonB7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB7.Click
        TakeHand(21)
    End Sub
    Private Sub ButtonB8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonB8.Click
        TakeHand(22)
    End Sub
    Private Sub ButtonRobo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRobo.Click
        RobotMove(-1)
    End Sub

    Private Sub Button82_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button82.Click
        UnmakeMove(modosi)
        DispAll()
    End Sub
End Class
