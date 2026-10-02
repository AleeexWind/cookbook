import { useEffect, useMemo, useState, type DragEvent } from 'react'
import { Link, Navigate } from 'react-router-dom'
import {
  clearSlot,
  exportShoppingListCsv,
  getMenuPlan,
  getRecipes,
  getShoppingList,
  placeRecipe,
  setPortions,
  type MenuPlan,
  type RecipeListItem,
  type ShoppingList,
} from '../api'
import { useAuth } from '../auth'
import { Layout } from '../components/Layout'

const DAYS = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'] as const
const MEALS = ['Breakfast', 'Lunch', 'Dinner'] as const

const DAY_RU: Record<string, string> = {
  Monday: 'Пн',
  Tuesday: 'Вт',
  Wednesday: 'Ср',
  Thursday: 'Чт',
  Friday: 'Пт',
  Saturday: 'Сб',
  Sunday: 'Вс',
}

const MEAL_RU: Record<string, string> = {
  Breakfast: 'Завтрак',
  Lunch: 'Обед',
  Dinner: 'Ужин',
}

export function MenuPlanPage() {
  const { token } = useAuth()
  const [plan, setPlan] = useState<MenuPlan | null>(null)
  const [recipes, setRecipes] = useState<RecipeListItem[]>([])
  const [list, setList] = useState<ShoppingList | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!token) return
    let cancelled = false
    ;(async () => {
      try {
        const [planData, recipeData] = await Promise.all([getMenuPlan(token), getRecipes(token)])
        if (!cancelled) {
          setPlan(planData)
          setRecipes(recipeData)
        }
      } catch (err) {
        if (!cancelled) setError(err instanceof Error ? err.message : 'Ошибка')
      }
    })()
    return () => {
      cancelled = true
    }
  }, [token])

  const slotMap = useMemo(() => {
    const map = new Map<string, { recipeId: string | null; recipeTitle: string | null }>()
    plan?.slots.forEach((slot) => map.set(`${slot.day}:${slot.meal}`, slot))
    return map
  }, [plan])

  const hasRecipes = plan?.slots.some((s) => s.recipeId) ?? false

  if (!token) return <Navigate to="/login" replace />

  async function onDrop(day: string, meal: string, event: DragEvent) {
    event.preventDefault()
    const recipeId = event.dataTransfer.getData('text/recipe-id')
    if (!recipeId || !token) return
    try {
      setPlan(await placeRecipe(token, day, meal, recipeId))
      setList(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  async function onClear(day: string, meal: string) {
    if (!token) return
    try {
      setPlan(await clearSlot(token, day, meal))
      if (list) {
        try {
          setList(await getShoppingList(token))
        } catch {
          setList(null)
        }
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  async function onGenerate() {
    if (!token) return
    try {
      setList(await getShoppingList(token))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка')
      setList(null)
    }
  }

  async function onExport() {
    if (!token) return
    try {
      const blob = await exportShoppingListCsv(token)
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = 'shopping-list.csv'
      a.click()
      URL.revokeObjectURL(url)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка')
    }
  }

  return (
    <Layout>
      <section className="page menu-page">
        <h1>План меню</h1>
        {error && <p className="error">{error}</p>}
        {!plan ? (
          <p>Загрузка...</p>
        ) : (
          <>
            <label className="portions">
              Порции на план
              <input
                type="number"
                min={1}
                max={20}
                value={plan.portions}
                onChange={async (e) => {
                  try {
                    setPlan(await setPortions(token, Number(e.target.value)))
                    setList(null)
                  } catch (err) {
                    setError(err instanceof Error ? err.message : 'Ошибка')
                  }
                }}
              />
            </label>

            <div className="menu-layout">
              <aside className="picker">
                <h2>Рецепты</h2>
                {recipes.map((recipe) => (
                  <div
                    key={recipe.id}
                    className="picker-item"
                    draggable
                    onDragStart={(e) => e.dataTransfer.setData('text/recipe-id', recipe.id)}
                  >
                    {recipe.title}
                  </div>
                ))}
              </aside>

              <div className="week-grid">
                <div className="week-header" />
                {DAYS.map((day) => (
                  <div key={day} className="week-header">
                    {DAY_RU[day]}
                  </div>
                ))}
                {MEALS.map((meal) => (
                  <div key={meal} className="meal-row-contents" style={{ display: 'contents' }}>
                    <div className="meal-label">{MEAL_RU[meal]}</div>
                    {DAYS.map((day) => {
                      const slot = slotMap.get(`${day}:${meal}`)
                      return (
                        <div
                          key={`${day}-${meal}`}
                          className="slot"
                          onDragOver={(e) => e.preventDefault()}
                          onDrop={(e) => onDrop(day, meal, e)}
                        >
                          {slot?.recipeTitle ? (
                            <>
                              <span>{slot.recipeTitle}</span>
                              <button type="button" className="linkish" onClick={() => onClear(day, meal)}>
                                убрать
                              </button>
                            </>
                          ) : (
                            <span className="empty">перетащите сюда</span>
                          )}
                        </div>
                      )
                    })}
                  </div>
                ))}
              </div>
            </div>

            <div className="shopping">
              <h2>Список покупок</h2>
              <div className="actions">
                <button type="button" disabled={!hasRecipes} onClick={onGenerate}>
                  Сгенерировать
                </button>
                <button type="button" disabled={!list || list.items.length === 0} onClick={onExport}>
                  Экспорт CSV
                </button>
              </div>
              {!hasRecipes && <p>Добавьте рецепты в план, чтобы собрать список покупок.</p>}
              {list && (
                <ul>
                  {list.items.map((item) => (
                    <li key={`${item.ingredient}-${item.unit}`}>
                      {item.ingredient}: {item.quantity} {item.unit}
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </>
        )}
        <p>
          <Link to="/">К рецептам</Link>
        </p>
      </section>
    </Layout>
  )
}
