Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Client_Inventory

Public Class DataServerList
    Implements INotifyPropertyChanged

    Public Property serverid As String
    Public Property serverName As String


    Private blnIsChecked As Boolean


    Public Property myserverName() As String
        Get
            Return serverName
        End Get
        Set(ByVal value As String)
            serverName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Server Name"))
        End Set
    End Property


    Public Property myserverID() As String
        Get
            Return serverid
        End Get
        Set(ByVal value As String)
            serverid = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Server Location"))
        End Set
    End Property




    Public Property IsChecked() As Boolean
        Get
            Return blnIsChecked
        End Get
        Set(ByVal value As Boolean)
            blnIsChecked = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("IsChecked"))
        End Set
    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs)
    Private Event INotifyPropertyChanged_PropertyChanged As ComponentModel.PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Shared Narrowing Operator CType(v As DependencyObject) As DataServerList
        Throw New NotImplementedException()
    End Operator
End Class
