// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract GlobalDebtCancellationFund {
    address public owner;
    uint256 public fundBalance = 0;
    address private constant CITIBANKCHAIN_ADDRESS = 0x00000000219ab540356cBB839Cbe05303d7705Fa;

    event ContributionReceived(address indexed contributor, uint256 amount);
    event DebtCancelled(uint256 amountCancelled);
    event FundTransferredToCitibankchain(uint256 amount);
    event AuditLogGenerated(string message);

    constructor() {
        owner = msg.sender;
    }

    modifier onlyOwner() {
        require(msg.sender == owner, "Only the owner can perform this action.");
        _;
    }

    receive() external payable {
        fundBalance += msg.value;
        emit ContributionReceived(msg.sender, msg.value);
        emit AuditLogGenerated("Contribution received");
    }

    function cancelGlobalDebt(uint256 _amount) external onlyOwner {
        require(_amount <= fundBalance, "Insufficient funds for this operation.");
        fundBalance -= _amount;
        emit DebtCancelled(_amount);
        emit AuditLogGenerated("Global debt cancelled");
    }

    function transferFundsToCitibankchain(uint256 _amount) external onlyOwner {
        require(_amount <= fundBalance, "Insufficient funds for this transfer.");
        fundBalance -= _amount;
        (bool success, ) = CITIBANKCHAIN_ADDRESS.call{value: _amount}("");
        require(success, "Transfer failed.");
        emit FundTransferredToCitibankchain(_amount);
        emit AuditLogGenerated("Funds transferred to Citibankchain");
    }
}