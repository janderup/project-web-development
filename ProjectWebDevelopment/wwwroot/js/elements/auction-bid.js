export class AuctionBid extends HTMLElement {
    
    constructor() {
        super();
    }
    
    connectedCallback() {
        const fullName = this.getAttribute('name');
        const price = this.getAttribute('price');

        this.innerHTML = `<div class="auction-bid d-flex justify-content-between"><div class="name">${fullName}</div><div class="bid">&euro; ${price}</div></div>`;
    }
}

customElements.define('auction-bid', AuctionBid);