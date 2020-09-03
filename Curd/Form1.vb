Imports System.Data.OleDb

Public Class Form1

    Public dataSet As DataSet


    'menampilkan table penjualan pada data grid
    Private Sub showTable(Optional ByVal keyword As String = "Null")

        'jika terdapat keyword pencarian
        If keyword <> "Null" Then
            dataAdapter = New OleDbDataAdapter("SELECT * FROM tPenjualan WHERE no_penjualan LIKE '%" & keyword & "%' OR nama_barang LIKE '%" & keyword & "%' OR harga_barang LIKE '%" & keyword & "%' ", connect)
        Else
            dataAdapter = New OleDbDataAdapter("SELECT * FROM tPenjualan", connect)

        End If

        dataSet = New DataSet
        dataAdapter.Fill(dataSet, "tPenjualan")
        dataGrid.DataSource = dataSet.Tables("tPenjualan")

    End Sub

    'membuat no penjualan secara otomatis
    Private Sub getNoPenjualan()

        Dim total As Integer
        Call connection()

        command = New OleDbCommand("SELECT COUNT(*) as total FROM tPenjualan", connect)
        total = command.ExecuteScalar

        'jika row di database tidak ada
        If total <> 0 Then

            'pengecekan jika lebih dari 10
            Select Case total
                Case Is <= 9
                    noPenjualan.Text = "P00" & total + 1
                Case Is <= 99
                    noPenjualan.Text = "P0" & total + 1
            End Select

        Else
            noPenjualan.Text = "P001"
        End If

    End Sub


    'reset form 
    Private Sub resetForm()

        inpNama.Text = ""
        inpHarga.Text = ""
        inpJumlah.Text = ""
        total.Text = "Rp.0"
        inpSearch.Text = ""

    End Sub


    'pertama kali di load
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Call connection()
        Call showTable()
        Call getNoPenjualan()

    End Sub


    'menambahkan data
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Call connection()

        command = New OleDbCommand("INSERT INTO tPenjualan (no_penjualan, nama_barang, harga_barang, jumlah_barang, total_harga) VALUES (@no_penjualan, @nama_barang, @harga_barang, @jumlah_barang, @total_harga) ", connect)

        'mengisi parameter pada insert
        command.Parameters.Add("@no_penjualan", OleDbType.Char).Value = noPenjualan.Text
        command.Parameters.Add("@nama_barang", OleDbType.VarChar).Value = inpNama.Text
        command.Parameters.Add("@harga_barang", OleDbType.Integer).Value = inpHarga.Text
        command.Parameters.Add("@jumlah_barang", OleDbType.Integer).Value = inpJumlah.Text
        command.Parameters.Add("@total_harga", OleDbType.Integer).Value = inpHarga.Text * inpJumlah.Text

        command.ExecuteNonQuery()
        MsgBox("Berhasil Menambahkan Data", MsgBoxStyle.Information)
        Call getNoPenjualan()
        Call showTable()
        Call resetForm()

    End Sub



    'pencarian data
    Private Sub inpSearch_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles inpSearch.KeyPress

        'Jika inputan lebih dari 3
        If inpSearch.Text <> "" Then
            'melakukan pencarian data memasukan keyword
            Call showTable(inpSearch.Text)
        Else
            'menampilkan semua data
            Call showTable()
        End If
    End Sub

    Private Sub dataGrid_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dataGrid.CellClick

        Dim id As String
        
        Call connection()

        id = dataGrid.Rows(e.RowIndex).Cells(0).Value
        command = New OleDbCommand("SELECT * FROM tPenjualan WHERE no_penjualan='" & id & "' ", connect)
        dataReader = command.ExecuteReader
        dataReader.Read()

        'menampikan result kedalam form
        noPenjualan.Text = dataReader.Item(0)
        inpNama.Text = dataReader.Item(1)
        inpHarga.Text = dataReader.Item(2)
        inpJumlah.Text = dataReader.Item(3)
        total.Text = "Rp." & dataReader.Item(4)

    End Sub


    Private Sub inpJumlah_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles inpJumlah.TextChanged
        Dim totalHarga As Double
        totalHarga = Val(inpHarga.Text) * Val(inpJumlah.Text)
        total.Text = "Rp." & totalHarga

    End Sub


    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Call resetForm()
    End Sub

    'menghapus data
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        Call connection()
        command = New OleDbCommand("DELETE FROM tPenjualan WHERE no_penjualan=@no_penjualan ", connect)
        command.Parameters.Add("@no_penjualan", OleDbType.Char).Value = noPenjualan.Text
        command.ExecuteNonQuery()

        MsgBox("Berhasil Menghapus Data")
        Call resetForm()
        Call getNoPenjualan()
        Call showTable()


    End Sub

    'Update Data
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        Call connection()

        command = New OleDbCommand("UPDATE tPenjualan SET nama_barang=@nama_barang, harga_barang=@harga_barang, jumlah_barang=@jumlah_barang, total_harga=@total_harga WHERE no_penjualan=@no_penjualan", connect)
        command.Parameters.AddWithValue("@nama_barang", inpNama.Text)
        command.Parameters.AddWithValue("@harga_barang", inpHarga.Text)
        command.Parameters.AddWithValue("@jumlah_barang", inpJumlah.Text)
        command.Parameters.AddWithValue("@total_harga", inpJumlah.Text * inpHarga.Text)
        command.Parameters.AddWithValue("@no_penjualan", noPenjualan.Text)

        command.ExecuteNonQuery()

        MsgBox("Berhasil Mengubah Data")
        Call getNoPenjualan()
        Call showTable()
        Call resetForm()
    End Sub

End Class

