// SPDX-License-Identifier: MIT
pragma solidity >=0.4.22 <0.9.0;

contract PayForItself {
    
    event Paid(address indexed _user, uint256 _amount);

    function payUser() public {
        require(address(this).balance > 0, "Contract has insufficient funds");

        uint256 amount = 1 ether; // Change to desired amount

        require(address(this).balance >= amount, "Contract has insufficient funds for this transaction");
        
        (bool success, ) = msg.sender.call{value: amount}("");
        require(success, "Failed to send Ether");

        emit Paid(msg.sender, amount);
    }

    // Allow contract to receive Ether
    receive() external payable {}
}