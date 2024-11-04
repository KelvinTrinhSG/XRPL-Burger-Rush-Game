using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Thirdweb;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopAndPlayManager : MonoBehaviour
{
    public string Address { get; private set; }

    private string receiverAddress = "0xA24d7ECD79B25CE6C66f1Db9e06b66Bd11632E00";

    string NFTAddressSmartContract = "0x00CFc51952Fb6327eaD8A1ad450F57c0DB28A365";
    string NFTAddressSmartContractSeat = "0x00CFc51952Fb6327eaD8A1ad450F57c0DB28A365";
    string NFTAddressSmartContractJukeBox = "0x86170f8B3E63E64E3b1F31e3b3f4878b5e2Ed4c2";
    string NFTAddressSmartContractFlowers = "0x1A8C7a6C9e0eFba20fB6b27989b1E68A6e7f6F4f";

    public Button shopButton;

    public Button seatButton;
    public Button jukeBoxButton;
    public Button flowersButton;
    public Button moneyButton;
    public Button backButton;

    public TMP_Text seatBalanceText;
    public TMP_Text jukeBoxBalanceText;
    public TMP_Text flowersBalanceText;
    public TMP_Text totalMoneyBoughtText;

    public TMP_Text costSeatButton;
    public TMP_Text costJukeBoxButton;
    public TMP_Text costFlowersButton;

    public TextMeshProUGUI buyingStatusText;

    public void PlayGame() {
        SceneManager.LoadScene("Init");
    }

    private void Start()
    {
        ResourceBoost.Instance.money = 0;
        shopButton.interactable = false;
        CheckNFTBalance();
    }

    public async void CheckNFTBalance() {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        var contractSeat = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractSeat);
        try
        {
            List<NFT> nftList = await contractSeat.ERC721.GetOwned(Address);
            if (nftList.Count == 0)
            {
                seatBalanceText.text = "Seat: 00";
            }
            else
            {
                costSeatButton.text = "Owned";
                seatBalanceText.text = "Seat: " + nftList.Count;
                PlayerPrefs.SetInt("shopItem-1", 1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
            // Handle the error, e.g., show an error message to the user or retry the operation
            shopButton.interactable = true;
        }

        var contractJukeBox = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractJukeBox);
        try
        {
            List<NFT> nftList = await contractJukeBox.ERC721.GetOwned(Address);
            if (nftList.Count == 0)
            {
                jukeBoxBalanceText.text = "JukeBox: 00";
            }
            else
            {
                costJukeBoxButton.text = "Owned";
                jukeBoxBalanceText.text = "JukeBox: " + nftList.Count;
                PlayerPrefs.SetInt("shopItem-2", 1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
            // Handle the error, e.g., show an error message to the user or retry the operation
            shopButton.interactable = true;
        }

        var contractFlowers = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractFlowers);
        try
        {
            List<NFT> nftList = await contractFlowers.ERC721.GetOwned(Address);
            if (nftList.Count == 0)
            {
                flowersBalanceText.text = "Flower: 00";
                shopButton.interactable = true;
            }
            else
            {
                costFlowersButton.text = "Owned";
                flowersBalanceText.text = "Flower: " + nftList.Count;
                shopButton.interactable = true;
                PlayerPrefs.SetInt("shopItem-3", 1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
            // Handle the error, e.g., show an error message to the user or retry the operation
            shopButton.interactable = true;
        }
    }

    private static float ConvertStringToFloat(string numberStr)
    {
        // Convert the string to a float
        float number = float.Parse(numberStr);

        // Return the float value
        return number;
    }

    public async void SpendTokenToBuyNFT(int indexValue)
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        seatButton.interactable = false;
        jukeBoxButton.interactable = false;
        flowersButton.interactable = false;
        moneyButton.interactable = false;
        backButton.interactable = false;

        float costValue = 0;
        buyingStatusText.text = "Buying...";
        buyingStatusText.gameObject.SetActive(true);
        if (indexValue == 0)
        {
            var contractSeat = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractSeat);
            try
            {
                List<NFT> nftList = await contractSeat.ERC721.GetOwned(Address);
                if (nftList.Count > 0)
                { 
                    costSeatButton.text = "Owned";
                    buyingStatusText.text = "You Owned this NFT";
                    buyingStatusText.gameObject.SetActive(true);
                    seatButton.interactable = true;
                    jukeBoxButton.interactable = true;
                    flowersButton.interactable = true;
                    moneyButton.interactable = true;
                    backButton.interactable = true;
                    PlayerPrefs.SetInt("shopItem-1", 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                // Handle the error, e.g., show an error message to the user or retry the operation
                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;
            }
            costValue = 0.1f;
            NFTAddressSmartContract = NFTAddressSmartContractSeat;
        }
        else if (indexValue == 1)
        {
            var contractJukeBox = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractJukeBox);
            try
            {
                List<NFT> nftList = await contractJukeBox.ERC721.GetOwned(Address);
                if (nftList.Count > 0)
                {                    
                    costJukeBoxButton.text = "Owned";
                    buyingStatusText.text = "You Owned this NFT";
                    buyingStatusText.gameObject.SetActive(true);
                    seatButton.interactable = true;
                    jukeBoxButton.interactable = true;
                    flowersButton.interactable = true;
                    moneyButton.interactable = true;
                    backButton.interactable = true;
                    PlayerPrefs.SetInt("shopItem-2", 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                // Handle the error, e.g., show an error message to the user or retry the operation
                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;
            }
            costValue = 0.2f;
            NFTAddressSmartContract = NFTAddressSmartContractJukeBox;
        }
        else if (indexValue == 2)
        {
            var contractFlowers = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContractFlowers);
            try
            {
                List<NFT> nftList = await contractFlowers.ERC721.GetOwned(Address);
                if (nftList.Count > 0)
                {                    
                    costFlowersButton.text = "Owned";
                    buyingStatusText.text = "You Owned this NFT";
                    buyingStatusText.gameObject.SetActive(true);
                    seatButton.interactable = true;
                    jukeBoxButton.interactable = true;
                    flowersButton.interactable = true;
                    moneyButton.interactable = true;
                    backButton.interactable = true;
                    PlayerPrefs.SetInt("shopItem-3", 1);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                // Handle the error, e.g., show an error message to the user or retry the operation
                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;
            }
            costValue = 0.3f;
            NFTAddressSmartContract = NFTAddressSmartContractFlowers;
        }

        var userBalance = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        if (ConvertStringToFloat(userBalance.displayValue) < costValue)
        {
            buyingStatusText.text = "Not Enough XRP";
        }
        else
        {
            //try
            //{
            //    // Thực hiện chuyển tiền, nếu thành công thì tiếp tục xử lý giao diện
            //    await ThirdwebManager.Instance.SDK.Wallet.Transfer(receiverAddress, costValue.ToString());

            // Chỉ thực hiện các thay đổi giao diện nếu chuyển tiền thành công
            var contract = ThirdwebManager.Instance.SDK.GetContract(NFTAddressSmartContract);
            try
            {
                var result = await contract.ERC721.ClaimTo(Address, 1);

                if (indexValue == 0)
                {
                    buyingStatusText.text = "+1 Seat";

                    try
                    {
                        List<NFT> nftList = await contract.ERC721.GetOwned(Address);
                        if (nftList.Count == 0)
                        {
                            seatBalanceText.text = "Seat: 00";
                        }
                        else
                        {
                            costSeatButton.text = "Owned";
                            seatButton.interactable = false;
                            seatBalanceText.text = "Seat: " + nftList.Count;
                            PlayerPrefs.SetInt("shopItem-1", 1);
                        }
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                        // Handle the error, e.g., show an error message to the user or retry the operation
                        buyingStatusText.text = "Failed to buy NFT. Please try again.";
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                }
                else if (indexValue == 1)
                {
                    buyingStatusText.text = "+1 JukeBox";
                    try
                    {
                        List<NFT> nftList = await contract.ERC721.GetOwned(Address);
                        if (nftList.Count == 0)
                        {

                            jukeBoxBalanceText.text = "JukeBox: 00";
                        }
                        else
                        {
                            costJukeBoxButton.text = "Owned";
                            jukeBoxButton.interactable = false;
                            jukeBoxBalanceText.text = "JukeBox: " + nftList.Count;
                            PlayerPrefs.SetInt("shopItem-2", 1);
                        }
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                        // Handle the error, e.g., show an error message to the user or retry the operation
                        buyingStatusText.text = "Failed to buy NFT. Please try again.";
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                }
                else if (indexValue == 2)
                {
                    buyingStatusText.text = "+1 flower";
                    try
                    {
                        List<NFT> nftList = await contract.ERC721.GetOwned(Address);
                        if (nftList.Count == 0)
                        {
                            flowersBalanceText.text = "FLower: 00";
                        }
                        else
                        {
                            costFlowersButton.text = "Owned";
                            flowersButton.interactable = false;
                            flowersBalanceText.text = "Flower: " + nftList.Count;
                            PlayerPrefs.SetInt("shopItem-3", 1);
                        }
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"An error occurred while fetching NFTs: {ex.Message}");
                        // Handle the error, e.g., show an error message to the user or retry the operation
                        buyingStatusText.text = "Failed to buy NFT. Please try again.";
                        seatButton.interactable = true;
                        jukeBoxButton.interactable = true;
                        flowersButton.interactable = true;
                        moneyButton.interactable = true;
                        backButton.interactable = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while buying the NFT: {ex.Message}");
                // Optionally, update the UI to inform the user of the error
                buyingStatusText.text = "Failed to buy NFT. Please try again.";
                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;
            }
            //}
            //catch (Exception ex)
            //{
            //    // Xử lý ngoại lệ nếu có lỗi xảy ra
            //    Debug.LogError($"Lỗi khi thực hiện chuyển tiền: {ex.Message}");
            //    buyingStatusText.text = "Error. Please try again";
            //    seatButton.interactable = true;
            //    jukeBoxButton.interactable = true;
            //    flowersButton.interactable = true;
            //    moneyButton.interactable = true;
            //    backButton.interactable = true;
            //}
        }
    }

    public async void SpendTokenToBuyMoney()
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        seatButton.interactable = false;
        jukeBoxButton.interactable = false;
        flowersButton.interactable = false;
        moneyButton.interactable = false;
        backButton.interactable = false;

        float costValue = 0.1f;
        buyingStatusText.text = "Buying...";
        buyingStatusText.gameObject.SetActive(true);       
        var userBalance = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        if (ConvertStringToFloat(userBalance.displayValue) < costValue)
        {
            buyingStatusText.text = "Not Enough XRP";
        }
        else
        {
            try
            {
                // Thực hiện chuyển tiền, nếu thành công thì tiếp tục xử lý giao diện
                await ThirdwebManager.Instance.SDK.Wallet.Transfer(receiverAddress, costValue.ToString());

                // Chỉ thực hiện các thay đổi giao diện nếu chuyển tiền thành công

                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;

                buyingStatusText.text = "+ $100";
                ResourceBoost.Instance.money += 100;
                totalMoneyBoughtText.text = "Total Money Bought: " + ResourceBoost.Instance.money;
                int playerMoney = PlayerPrefs.GetInt("PlayerMoney", 0); // 100 is the default value if "PlayerMoney" doesn't exist
                PlayerPrefs.SetInt("PlayerMoney", playerMoney + 100);

            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Debug.LogError($"Lỗi khi thực hiện chuyển tiền: {ex.Message}");
                buyingStatusText.text = "Error. Please try again";
                seatButton.interactable = true;
                jukeBoxButton.interactable = true;
                flowersButton.interactable = true;
                moneyButton.interactable = true;
                backButton.interactable = true;
            }
        }
    }


}
