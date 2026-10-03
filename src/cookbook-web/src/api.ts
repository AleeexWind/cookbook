const API_BASE = import.meta.env.VITE_API_BASE ?? ''

export type AuthResult = {
  userId: string
  userName: string
  accessToken: string
}

export type RecipeListItem = {
  id: string
  title: string
  description: string
  cookingTimeMinutes: number
  difficulty: string
  photo: string
  category: string
  tags: string[]
  averageRating: number | null
  commentsCount: number
  isFavourite: boolean | null
}

export type CommentDto = {
  id: string
  authorName: string
  text: string
  postedAt: string
}

export type RecipeDetails = {
  id: string
  title: string
  description: string
  cookingTimeMinutes: number
  difficulty: string
  photo: string
  category: string
  tags: string[]
  averageRating: number | null
  visibility: string
  authorName: string
  portions: number
  ingredients: { name: string; quantity: number; unit: string }[]
  steps: { order: number; instruction: string }[]
  comments: CommentDto[]
  isFavourite: boolean | null
}

export type MenuPlan = {
  portions: number
  slots: {
    day: string
    meal: string
    recipeId: string | null
    recipeTitle: string | null
  }[]
}

export type ShoppingList = {
  items: { ingredient: string; quantity: number; unit: string }[]
}

function authHeader(token: string | null): HeadersInit {
  return token ? { Authorization: `Bearer ${token}` } : {}
}

async function parseError(response: Response): Promise<string> {
  try {
    const data = await response.json()
    return data.error ?? response.statusText
  } catch {
    return response.statusText
  }
}

export async function login(userName: string, password: string): Promise<AuthResult> {
  const response = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userName, password }),
  })
  if (!response.ok) throw new Error('Неверный логин или пароль')
  return response.json()
}

export async function register(userName: string, password: string): Promise<AuthResult> {
  const response = await fetch(`${API_BASE}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userName, password }),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function getRecipes(token: string | null, search = ''): Promise<RecipeListItem[]> {
  const query = search ? `?search=${encodeURIComponent(search)}` : ''
  const response = await fetch(`${API_BASE}/api/recipes${query}`, {
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function getRecipe(token: string | null, id: string, portions?: number): Promise<RecipeDetails> {
  const query = portions ? `?portions=${portions}` : ''
  const response = await fetch(`${API_BASE}/api/recipes/${id}${query}`, {
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function setFavourite(token: string, id: string, isActive: boolean): Promise<void> {
  const response = await fetch(`${API_BASE}/api/recipes/${id}/favourite`, {
    method: 'PUT',
    headers: { ...authHeader(token), 'Content-Type': 'application/json' },
    body: JSON.stringify({ isActive }),
  })
  if (!response.ok) throw new Error(await parseError(response))
}

export async function setRating(token: string, id: string, stars: number): Promise<void> {
  const response = await fetch(`${API_BASE}/api/recipes/${id}/rating`, {
    method: 'PUT',
    headers: { ...authHeader(token), 'Content-Type': 'application/json' },
    body: JSON.stringify({ stars }),
  })
  if (!response.ok) throw new Error(await parseError(response))
}

export async function addComment(token: string, id: string, text: string): Promise<CommentDto> {
  const response = await fetch(`${API_BASE}/api/recipes/${id}/comments`, {
    method: 'POST',
    headers: { ...authHeader(token), 'Content-Type': 'application/json' },
    body: JSON.stringify({ text }),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function deleteComment(token: string, recipeId: string, commentId: string): Promise<void> {
  const response = await fetch(`${API_BASE}/api/recipes/${recipeId}/comments/${commentId}`, {
    method: 'DELETE',
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
}

export async function getMenuPlan(token: string): Promise<MenuPlan> {
  const response = await fetch(`${API_BASE}/api/menu-plan`, { headers: authHeader(token) })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function setPortions(token: string, portions: number): Promise<MenuPlan> {
  const response = await fetch(`${API_BASE}/api/menu-plan/portions`, {
    method: 'PUT',
    headers: { ...authHeader(token), 'Content-Type': 'application/json' },
    body: JSON.stringify({ portions }),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function placeRecipe(token: string, day: string, meal: string, recipeId: string): Promise<MenuPlan> {
  const response = await fetch(`${API_BASE}/api/menu-plan/slots/${day}/${meal}`, {
    method: 'PUT',
    headers: { ...authHeader(token), 'Content-Type': 'application/json' },
    body: JSON.stringify({ recipeId }),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function clearSlot(token: string, day: string, meal: string): Promise<MenuPlan> {
  const response = await fetch(`${API_BASE}/api/menu-plan/slots/${day}/${meal}`, {
    method: 'DELETE',
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function getShoppingList(token: string): Promise<ShoppingList> {
  const response = await fetch(`${API_BASE}/api/menu-plan/shopping-list`, {
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.json()
}

export async function exportShoppingListCsv(token: string): Promise<Blob> {
  const response = await fetch(`${API_BASE}/api/menu-plan/shopping-list/export`, {
    headers: authHeader(token),
  })
  if (!response.ok) throw new Error(await parseError(response))
  return response.blob()
}

export function photoUrl(fileName: string): string {
  return `${API_BASE}/api/photos/${encodeURIComponent(fileName)}`
}
