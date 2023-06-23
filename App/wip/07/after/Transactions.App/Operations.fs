﻿module Transactions.Operations

open System

open Transactions.Domain

module Accounts =
    let private calculateBalance transactions =
        let getAdjustedAmount transaction =
            match transaction.Type with
            | Withdraw -> -transaction.Amount
            | Deposit -> transaction.Amount
        transactions |> List.map getAdjustedAmount |>  List.sum
        
    let private getNextTransactionId (transactions:Transaction list) =
        match transactions with
        | [] -> 0
        | _ -> transactions |> List.map (fun t -> t.Id) |> List.max |> (+) 1
(*
    let private deposit amount account=
        let transaction = { Id = getNextTransactionId account.Transactions; Type = Deposit; Amount = amount }
        let transactions = account.Transactions @ [transaction]
        { 
            account with 
                Balance = calculateBalance transactions
                Transactions = transactions
        }

    let private withdraw amount account =
        let transaction = { Id = getNextTransactionId account.Transactions; Type = Withdraw; Amount = amount }
        let transactions = account.Transactions @ [transaction]
        { 
            account with 
                Balance = calculateBalance transactions
                Transactions = transactions
        }
*)
    let private apply transactionType amount account =
        let transaction = { Id = getNextTransactionId account.Transactions; Type = transactionType; Amount = amount }
        let transactions = account.Transactions @ [transaction]
        { 
            account with 
                Balance = calculateBalance transactions
                Transactions = transactions
        }

        
    module Commands =
        type AccountCommands = | Withdraw of int * decimal | Deposit of int * decimal | Invalid

    module Processors =
        open Commands

        let execute command =
            match command with
                | Withdraw (accountId, amount) ->
                    int accountId |> Repository.Account.get |> apply TransactionType.Withdraw amount |> Repository.Account.put
                    true
                | Deposit (accountId, amount) ->
                    int accountId |> Repository.Account.get |> apply TransactionType.Deposit amount |> Repository.Account.put
                    true
                | Invalid -> false

                    

