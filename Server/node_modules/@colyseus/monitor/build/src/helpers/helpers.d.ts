import type { MonitorOptions } from "../../";
export type ExtractStringNames<T> = T extends (infer U)[] ? U extends string ? U : never : never;
export declare const valueFormatter: {
    [key in ExtractStringNames<MonitorOptions['columns']>]?: Function;
};
export declare function humanizeElapsedTime(milliseconds: number): string;
