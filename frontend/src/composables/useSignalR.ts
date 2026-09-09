import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr'
import { useAuthStore } from '@/stores/auth'

let connection: HubConnection | undefined
export function useSignalR() {
  const auth = useAuthStore()
  function connect() {
    connection ??= new HubConnectionBuilder().withUrl(`${import.meta.env.VITE_SIGNALR_HUB_URL ?? 'http://localhost:5000'}/hubs/agreements`, { accessTokenFactory: () => auth.token ?? '' }).withAutomaticReconnect().build()
    return connection.start().catch(() => undefined)
  }
  return { connect, on: (event: string, handler: (...args: unknown[]) => void) => connection?.on(event, handler), off: (event: string, handler: (...args: unknown[]) => void) => connection?.off(event, handler) }
}
