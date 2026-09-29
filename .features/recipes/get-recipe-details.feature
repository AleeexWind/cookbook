Feature: Get recipe details
    As a user I want to see full recipe details including characteristics and ingredients
    so that I know what the recipe is and what I need to cook

    Scenario: Guest views recipe details without favourites mark
        Given There is a public recipe "Pasta Carbonara" for 2 portions in the database
        And The recipe has the following ingredients
        And The recipe has the following comments
        And I am not logged in
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should see title, description, cooking time, difficulty, photo, category, tags, rating, visibility and author
        And I should see ingredients with name, quantity, unit and portions set to 2
        And I should see comments ordered from newest to oldest with author name, text and posted at
        And I should not see the favourites mark

    Examples:
       | Title           | Description           | Cooking time | Difficulty | Photo         | Category | Tags  | Rating | Visibility | Author | Portions |
       | Pasta Carbonara | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner   | quick | 4.5    | public     | alice  | 2        |

    Examples:
       | Ingredient | Quantity | Unit  |
       | Spaghetti  | 200      | g     |
       | Eggs       | 2        | units |
       | Bacon      | 100      | g     |
       | Milk       | 50       | ml    |

    Examples:
       | Author | Text            | Posted at        |
       | bob    | Looks delicious  | 2026-09-28 18:00 |
       | alice  | Family favorite  | 2026-09-27 12:00 |
       | bob    | Made it twice    | 2026-09-26 09:00 |

    Scenario: Authorized user views recipe details with favourites mark
        Given There is a public recipe "Pasta Carbonara" for 2 portions in the database
        And The recipe has the following ingredients
        And The recipe has the following comments
        And I am logged in as "bob"
        And I have favourited the recipe "Pasta Carbonara"
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should see title, description, cooking time, difficulty, photo, category, tags, rating, visibility and author
        And I should see ingredients with name, quantity, unit and portions set to 2
        And I should see comments ordered from newest to oldest with author name, text and posted at
        And I should see the favourites mark as active

    Examples:
       | Title           | Description           | Cooking time | Difficulty | Photo         | Category | Tags  | Rating | Visibility | Author | Portions | Favourite |
       | Pasta Carbonara | Classic Italian pasta | 30           | Easy       | carbonara.jpg | dinner   | quick | 4.5    | public     | alice  | 2        | active    |

    Examples:
       | Ingredient | Quantity | Unit  |
       | Spaghetti  | 200      | g     |
       | Eggs       | 2        | units |
       | Bacon      | 100      | g     |
       | Milk       | 50       | ml    |

    Examples:
       | Author | Text            | Posted at        |
       | bob    | Looks delicious  | 2026-09-28 18:00 |
       | alice  | Family favorite  | 2026-09-27 12:00 |
       | bob    | Made it twice    | 2026-09-26 09:00 |

    Scenario: Scale ingredients when portions change
        Given There is a public recipe "Pasta Carbonara" for 2 portions in the database
        And The recipe has the following ingredients for 2 portions
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I set portions to 4
        Then I should see ingredient quantities scaled for 4 portions

    Examples:
       | Ingredient | Quantity for 2 portions | Quantity for 4 portions | Unit  |
       | Spaghetti  | 200                     | 400                     | g     |
       | Eggs       | 2                       | 4                       | units |
       | Bacon      | 100                     | 200                     | g     |
       | Milk       | 50                      | 100                     | ml    |
