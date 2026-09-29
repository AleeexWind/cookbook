Feature: Recipe favourites
    As an authorized user I want to activate and deactivate the favourites star
    so that I can bookmark recipes from the list and details pages

    Scenario: Activate favourites star from the recipes list
        Given There is a public recipe "Pasta Carbonara"
        And I am logged in as "bob"
        And The recipe "Pasta Carbonara" is not in my favourites
        When I am on the recipes page
        And I activate the favourites star for "Pasta Carbonara"
        Then The favourites star for "Pasta Carbonara" should be active

    Scenario: Deactivate favourites star from the recipes list
        Given There is a public recipe "Pasta Carbonara"
        And I am logged in as "bob"
        And I have favourited the recipe "Pasta Carbonara"
        When I am on the recipes page
        And I deactivate the favourites star for "Pasta Carbonara"
        Then The favourites star for "Pasta Carbonara" should be inactive

    Scenario: Activate favourites star from recipe details
        Given There is a public recipe "Pasta Carbonara"
        And I am logged in as "bob"
        And The recipe "Pasta Carbonara" is not in my favourites
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I activate the favourites star
        Then The favourites star should be active

    Scenario: Deactivate favourites star from recipe details
        Given There is a public recipe "Pasta Carbonara"
        And I am logged in as "bob"
        And I have favourited the recipe "Pasta Carbonara"
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I deactivate the favourites star
        Then The favourites star should be inactive

    Scenario: Guest cannot activate or deactivate the favourites star
        Given There is a public recipe "Pasta Carbonara"
        And I am not logged in
        When I am on the recipes page
        Then I should not see the favourites star
        And I should not be able to activate or deactivate favourites
