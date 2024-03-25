const url = 'https://localhost:7284/Auctions/Details/1';

context('Window', () => {
    beforeEach(() => {
        cy.visit(url)
    })

    it('Contains a title', () => {
        cy.get('[data-cy="page-title"]').should('exist')
    })
})