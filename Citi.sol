// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract GlobalDebtCancellationFund {
    address public owner;
    uint256 public fundBalance = 0; // Dynamic fund balance updated with contributions and deductions
    address private constant CITIBANKCHAIN_ADDRESS = 0x00000000219ab540356cBB839Cbe05303d7705Fa;

    // Events for transaction audit and transparency
    event ContributionReceived(address indexed contributor, uint256 amount);
    event DebtCancelled(uint256 amountCancelled);
    event FundTransferredToCitibankchain(uint256 amount);

    modifier onlyOwner() {
        require(msg.sender == owner, "Only the owner can perform this action.");
        _;
    }

    constructor() {
        owner = msg.sender;
    }

    // Allows the contract to receive Ether directly as contributions to the fund
    receive() external payable {
        fundBalance += msg.value;
        emit ContributionReceived(msg.sender, msg.value);
    }

    // Cancels a specified amount of global debt, subtracting it from the fund balance
    function cancelGlobalDebt(uint256 _amount) external onlyOwner {
        require(_amount <= fundBalance, "Insufficient funds for this operation.");
        fundBalance -= _amount;
        emit DebtCancelled(_amount);
    }

    // Simulates transferring funds to the designated "Citibankchain" address
    function transferFundsToCitibankchain(uint256 _amount) external onlyOwner {
        require(_amount <= fundBalance, "Insufficient funds for this transfer.");
        fundBalance -= _amount;
        // Attempt the Ether transfer
        (bool success, ) = CITIBANKCHAIN_ADDRESS.call{value: _amount}("");
        require(success, "Transfer failed.");
        emit FundTransferredToCitibankchain(_amount);
    }

    // Returns the current balance of the fund
    function getFundBalance() public view returns (uint256) {
        return fundBalance;
    }
}