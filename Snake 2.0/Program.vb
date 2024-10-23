Imports System
Imports System.ComponentModel.Design
Imports System.Data
Imports System.Threading.Tasks.Dataflow

Module Program
    Public Structure HeadPos
        Public x As Integer
        Public y As Integer
    End Structure

    Public previousHeadPositions As New List(Of HeadPos)
    Public fruitX, fruitY As Integer

    Sub Main(args As String())
        Randomize()
        Dim grid() As Integer = {30, 10}

        Console.CursorVisible = False

        If grid(0) Mod 2 <> 0 Then
            grid(0) += 1
        End If
        If grid(1) Mod 2 = 0 Then
            grid(1) += 1
        End If

        Dim midPoint() As Integer = {(grid(0) + 1) / 2, (grid(1) + 1) / 2}
        DrawBorders(grid)

        Dim headX, headY, prevHeadX, prevHeadY As Integer
        headX = midPoint(0)
        headY = midPoint(1)
        PlaceHead(headX, headY)

        Dim keyPressed As ConsoleKey
        Dim lastKey As ConsoleKey = ConsoleKey.W
        Dim direction As String = "u"
        Dim gameOver As Boolean = False
        Dim previousPosition As New HeadPos
        Dim tailLength As Integer = 20
        Dim fruitExists As Boolean = False
        Dim score As Integer = 0

        Do
            prevHeadX = headX
            prevHeadY = headY

            If fruitExists = False Then
                fruitExists = Fruit(grid, prevHeadX, prevHeadY)
            End If

            If headX - 1 = fruitX And headY = fruitY Then
                fruitExists = False
                score += 1
                tailLength += 1
                fruitX = Nothing
                fruitY = Nothing
            End If

            Console.SetCursorPosition(grid(0) + 3, 0)
            Console.ForegroundColor = ConsoleColor.DarkYellow
            Console.Write($"SCORE: {score} points")
            Console.SetCursorPosition(grid(0) + 3, 1)
            Console.Write($"{fruitX}, {fruitY}              ")
            Console.SetCursorPosition(grid(0) + 3, 2)
            Try
                Console.Write(previousHeadPositions(previousHeadPositions.Count - 1))
            Catch ex As Exception

            End Try
            Console.ResetColor()


            Threading.Thread.Sleep(500)
            If Console.KeyAvailable Then
                keyPressed = Console.ReadKey(True).Key
                ClearInputBuffer()
                lastKey = keyPressed
            ElseIf lastKey <> Nothing Then
                keyPressed = lastKey
            End If
            Select Case keyPressed
                Case ConsoleKey.W
                    headY -= 1
                    direction = "u"
                Case ConsoleKey.S
                    headY += 1
                    direction = "d"
                Case ConsoleKey.A
                    headX -= 2
                    direction = "l"
                Case ConsoleKey.D
                    headX += 2
                    direction = "r"
                Case ConsoleKey.Escape
                    Exit Do
                Case Else
                    If direction = "u" Then
                        headY -= 1
                    ElseIf direction = "d" Then
                        headY += 1
                    ElseIf direction = "l" Then
                        headX -= 2
                    ElseIf direction = "r" Then
                        headX += 2
                    End If
            End Select
            gameOver = CheckWallCollision(headX, headY, grid, direction)

            If gameOver = True Then
                Exit Do
            End If

            gameOver = CheckSelfCollision(headX, headY, grid, direction)

            If gameOver = True Then
                Exit Do
            End If

            previousPosition.x = prevHeadX
            previousPosition.y = prevHeadY
            previousHeadPositions.Add(previousPosition)
            MoveHead(headX, headY, prevHeadX, prevHeadY, tailLength)
            previousPosition.x = Nothing
            previousPosition.y = Nothing
        Loop Until gameOver = True

        Console.SetCursorPosition(0, grid(1) + 2)
        Console.WriteLine($"Game Over with {score} points!")
        For Each thing In previousHeadPositions
            Console.WriteLine($"{thing.x}, {thing.y}")
        Next
    End Sub

    Sub DrawBorders(grid() As Integer)
        For i = 0 To grid(0) + 1
            Console.SetCursorPosition(i, 0)
            Console.Write("-")
        Next
        For i = 0 To grid(0) + 1
            Console.SetCursorPosition(i, grid(1) + 1)
            Console.Write("-")
        Next
        For i = 0 To grid(1) + 1
            Console.SetCursorPosition(0, i)
            Console.Write("|")
        Next
        For i = 0 To grid(1) + 1
            Console.SetCursorPosition(grid(0) + 1, i)
            Console.Write("|")
        Next
        Console.SetCursorPosition(0, 0)
        Console.Write("+")
        Console.SetCursorPosition(0, grid(1) + 1)
        Console.Write("+")
        Console.SetCursorPosition(grid(0) + 1, 0)
        Console.Write("+")
        Console.SetCursorPosition(grid(0) + 1, grid(1) + 1)
        Console.Write("+")
    End Sub

    Sub PlaceHead(headX As Integer, headY As Integer)
        Console.SetCursorPosition(headX - 1, headY)
        Console.BackgroundColor = ConsoleColor.Red
        Console.Write("  ")
        Console.ResetColor()
    End Sub

    Sub RemoveHead(headX As Integer, headY As Integer, prevHeadX As Integer, prevHeadY As Integer)
        Console.SetCursorPosition(prevHeadX - 1, prevHeadY)
        Console.ResetColor()
        Console.Write("  ")
        Console.SetCursorPosition(headX + 1, headY)
    End Sub

    Sub MoveHead(headX As Integer, headY As Integer, prevHeadX As Integer, prevHeadY As Integer, tailLength As Integer)
        PlaceHead(headX, headY)
        RemoveHead(headX, headY, prevHeadX, prevHeadY)
        PlaceTail(tailLength)
    End Sub

    Sub ClearInputBuffer()
        While Console.KeyAvailable
            Console.ReadKey(True)
        End While
    End Sub

    Function CheckWallCollision(headX As Integer, headY As Integer, grid() As Integer, direction As String)
        If direction = "u" AndAlso headY <= 0 Then
            Return True
        ElseIf direction = "d" AndAlso headY > grid(1) Then
            Return True
        ElseIf direction = "l" AndAlso headX <= 0 Then
            Return True
        ElseIf direction = "r" AndAlso headX > grid(0) Then
            Return True
        Else
            Return False
        End If
    End Function

    Function CheckSelfCollision(headX As Integer, headY As Integer, grid() As Integer, direction As String)
        For value = 0 To previousHeadPositions.Count - 1
            If previousHeadPositions(value).x = headX AndAlso previousHeadPositions(value).y = headY Then
                Return True
            End If
        Next
        Return False
    End Function

    Sub PlaceTail(tailLength As Integer)
        tailLength -= 1

        For Each value In previousHeadPositions
            Console.SetCursorPosition(value.x - 1, value.y)
            Console.ResetColor()
            Console.Write("  ")
        Next

        If previousHeadPositions.Count > tailLength Then
            previousHeadPositions.RemoveRange(0, previousHeadPositions.Count - tailLength - 1)
        End If

        For i = 0 To previousHeadPositions.Count - 1
            Console.SetCursorPosition(previousHeadPositions(i).x - 1, previousHeadPositions(i).y)
            If i Mod 2 = 0 Then
                Console.BackgroundColor = ConsoleColor.Green
            Else
                Console.BackgroundColor = ConsoleColor.DarkGreen
            End If
            Console.Write("  ")
        Next

        Console.ResetColor()
    End Sub

    Function Fruit(grid() As Integer, headX As Integer, headY As Integer)
        Dim success As Boolean = True
        Do
            fruitX = Int(Rnd() * (grid(0) - 2)) + 1
            fruitY = Int(Rnd() * (grid(1) - 2)) + 1

            If fruitX Mod 2 = 0 Then
                fruitX += 1
            End If

            success = True

            If fruitX = headX AndAlso fruitY = headY Then
                success = False
            End If

            For Each position In previousHeadPositions
                If position.x = fruitX + 1 AndAlso position.y = fruitY Then
                    success = False
                    Exit For
                End If
            Next
        Loop Until success

        Console.SetCursorPosition(fruitX, fruitY)
        Console.BackgroundColor = ConsoleColor.Magenta
        Console.Write("  ")
        Console.ResetColor()

        Return True
    End Function
End Module
